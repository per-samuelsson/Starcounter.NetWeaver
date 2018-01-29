
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Starcounter.Weaver {

    public class RoutedMethodImplementation {
        readonly CodeEmissionContext emitContext;
        readonly InterfaceImplementation interfaceImplementation;
        readonly MethodDefinition interfaceMethod;
        readonly MethodReference interfaceMethodRef;
        readonly MethodDefinition targetMethod;
        readonly MethodReference targetMethodRef;
        readonly TypeReference returnTypeRef;
        readonly string methodName;
        readonly MethodAttributes methodAttributes;
        readonly IEnumerable<ParameterDefinition> parameters;

        public MethodDefinition ImplementedMethod {
            get;
            private set;
        }
        
        public RoutedMethodImplementation(CodeEmissionContext codeEmissionContext, InterfaceImplementation implementation, MethodDefinition interfaceDefinition, MethodDefinition targetDefinition) {
            Guard.NotNull(codeEmissionContext, nameof(codeEmissionContext));
            Guard.NotNull(implementation, nameof(implementation));
            Guard.NotNull(interfaceDefinition, nameof(interfaceDefinition));
            Guard.NotNull(targetDefinition, nameof(targetDefinition));

            emitContext = codeEmissionContext;
            interfaceImplementation = implementation;
            interfaceMethod = interfaceDefinition;
            targetMethod = targetDefinition;
            
            interfaceMethodRef = emitContext.Use(interfaceMethod);
            returnTypeRef = emitContext.Use(interfaceMethod.ReturnType);
            targetMethodRef = emitContext.Use(targetMethod);

            parameters = interfaceMethod.Parameters.Select(p => new ParameterDefinition(
                p.Name, 
                p.Attributes,
                emitContext.Use(p.ParameterType)
                ));
            
            methodName = interfaceImplementation.InterfaceType.FullName + "." + interfaceMethod.Name;
            methodAttributes = interfaceMethod.Attributes;
            methodAttributes ^= MethodAttributes.Abstract;
            methodAttributes ^= MethodAttributes.Public;
            methodAttributes |= (MethodAttributes.Virtual | MethodAttributes.Private | MethodAttributes.HideBySig | MethodAttributes.Final);
        }

        public void ImplementOn(TypeDefinition type) {
            Guard.NotNull(type, nameof(type));
            if (!emitContext.Defines(type)) {
                throw new ArgumentException($"Type is not defined in expected module {emitContext.Module.Name}", type.FullName);
            }
            
            var methodImpl = new MethodDefinition(methodName, methodAttributes, returnTypeRef);
            foreach (var interfaceParameter in parameters) {
                methodImpl.Parameters.Add(interfaceParameter);
            }
            methodImpl.Overrides.Add(interfaceMethodRef);

            methodImpl.Body = new MethodBody(methodImpl);
            var il = methodImpl.Body.GetILProcessor();
            il.Append(il.Create(OpCodes.Ldarg_0));
            foreach (var p in methodImpl.Parameters) {
                il.Append(il.Create(OpCodes.Ldarg, p));
            }
            il.Append(il.Create(OpCodes.Call, targetMethodRef));
            il.Append(il.Create(OpCodes.Ret));

            // Make sure we optimize parameter stack loading on what
            // we generate
            // https://stackoverflow.com/questions/10231409/creating-an-il-instruction-with-an-inline-argument-using-mono-cecil
            il.Body.OptimizeMacros();

            type.Methods.Add(methodImpl);

            ImplementedMethod = methodImpl;
        }
    }
}
