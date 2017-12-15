
using Mono.Cecil;
using Mono.Cecil.Cil;
using Starcounter.Weaver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace starweave.Weaver {

    public sealed class DatabaseTypeConstructorRewriter {
        readonly IWeaverHost host;
        readonly CodeEmissionContext context;
        readonly ConstructorSignatureTypes signatureTypes;
        readonly ProxyConstructorEmitter proxyEmit;
        readonly ReplacementConstructorEmitter replacementEmit;
        readonly InsertConstructorEmitter insertEmit;

        /// <summary>
        /// This constructor define the contract of what is needed for all constructor
        /// rewriging in terms of contract from the target runtime.
        /// </summary>
        /// <param name="weaverHost"></param>
        /// <param name="emitContext"></param>
        /// <param name="proxyConstructorParameterType"></param>
        /// <param name="insertConstructorParameterType"></param>
        /// <param name="insertMethod"></param>
        public DatabaseTypeConstructorRewriter(IWeaverHost weaverHost, CodeEmissionContext emitContext, Type proxyConstructorParameterType, Type insertConstructorParameterType, MethodInfo insertMethod) {
            host = weaverHost ?? throw new ArgumentNullException(nameof(weaverHost));
            context = emitContext ?? throw new ArgumentNullException(nameof(emitContext));
            signatureTypes = new ConstructorSignatureTypes(emitContext, proxyConstructorParameterType, insertConstructorParameterType);
            
            proxyEmit = new ProxyConstructorEmitter(emitContext, signatureTypes.ProxyConstructorParameter);
            replacementEmit = new ReplacementConstructorEmitter(emitContext, signatureTypes.ProxyConstructorParameter);
            
            var insert = emitContext.Use(insertMethod);
            insertEmit = new InsertConstructorEmitter(emitContext, signatureTypes.InsertConstructorParameter, insert);
        }
        
        public void Rewrite(TypeDefinition typeDefinition, TypeDefinition baseTypeDefinition, DatabaseTypeState typeState) {
            // Gather ingoing instance constructors before we do any tampering.
            // Verify there are only originals and no constructors with signatures.
            // If there are, type is already weaved (internal error) or, more likely,
            // user has defined a constructor with one of our reserved types.
            var constructorSet = ConstructorSet.Discover(signatureTypes, typeDefinition);
            if (constructorSet.ContainConstructorsWithSignatureTypes) {
                throw new ArgumentException($"Type contain constructors with signature types.");
            }
            
            var redirects = new Dictionary<MethodReference, MethodReference>();
            
            if (baseTypeDefinition == null) {
                proxyEmit.Emit(typeDefinition);
                var insertConstructor = insertEmit.Emit(typeDefinition, typeState);
                redirects.Add(context.Module.GetObjectConstructorReference(), insertConstructor);
            }
            else {
                // Could be candidate for caching: consider case with lots of classes
                // deriving a single, same base.
                // TODO:
                var baseConstructorSet = ConstructorSet.Discover(signatureTypes, baseTypeDefinition);
                
                foreach (var baseReplacement in baseConstructorSet.GetReplacementMap()) {
                    redirects.Add(baseReplacement.Key, baseReplacement.Value);
                }

                var baseProxy = baseConstructorSet.ProxyConstructor;
                proxyEmit.Emit(typeDefinition, baseProxy);
            }

            var instanceConstructors = constructorSet.OriginalConstructors.ToArray();
            var replacementConstructors = new List<MethodDefinition>(instanceConstructors.Length);

            foreach (var instanceConstructor in instanceConstructors) {
                var replacementConstructor = replacementEmit.Emit(instanceConstructor);
                replacementEmit.Redirect(instanceConstructor, replacementConstructor, typeState);

                replacementConstructors.Add(replacementConstructor);
                redirects.Add(instanceConstructor, replacementConstructor);
            }
            
            foreach (var replacementConstructor in replacementConstructors) {
                var call = MethodCallFinder.FindSingleCallToAnyTarget(replacementConstructor, redirects.Keys);
                if (call == null) {
                    throw new Exception($"Constructor {replacementConstructor.FullName} contain no constructor call. We didn't expect that.");
                }
                
                var callTarget = (MethodReference)call.Operand;
                var replacement = redirects.First(pair => pair.Key.ReferenceSameMethod(callTarget)).Value;

                var il = replacementConstructor.Body.GetILProcessor();
                var loadsignature = il.Create(OpCodes.Ldnull);
                var loadhandle = il.Create(OpCodes.Ldarg, replacement.Parameters.Count - 1);
                var replacementCall = il.Create(call.OpCode, replacement);
                il.Replace(call, replacementCall);
                il.InsertBefore(replacementCall, loadsignature);
                il.InsertBefore(loadsignature, loadhandle);
            }
        }
    }
}