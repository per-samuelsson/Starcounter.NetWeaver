
using Mono.Cecil;
using Mono.Cecil.Cil;
using Starcounter.Weaver;
using System;

namespace starweave.Weaver {

    /// <summary>
    /// Replacement constructors are proxy constructors emitted by the weaver,
    /// where we assure they always end up in a single call to the insert
    /// constructor.
    /// </summary>
    public sealed class ReplacementConstructorEmitter {
        readonly CodeEmissionContext context;
        readonly TypeReference uniqueSignatureType;

        //
        // Foo(int i) {
        //   Console.Write(i);
        // }
        //
        // =>
        //
        // Foo(int i) : this(i, Foo.CreateHandle, [SignatureType]null) {
        // }
        //
        // Foo(int i, ulong handle, [SignatureType] dummy) {
        //   Console.Write(i);
        // }

        public ReplacementConstructorEmitter(CodeEmissionContext emitContext, TypeReference uniqueSignatureTypeRef) {
            uniqueSignatureType = uniqueSignatureTypeRef ?? throw new ArgumentNullException(nameof(uniqueSignatureTypeRef));
            context = emitContext ?? throw new ArgumentNullException(nameof(emitContext));
        }

        public static bool IsReplacementFor(MethodDefinition replacement, MethodDefinition original) {
            Guard.NotNull(replacement, nameof(replacement));
            Guard.NotNull(original, nameof(original));

            return false;
        }

        public MethodDefinition Emit(MethodDefinition original) {
            Guard.NotNull(original, nameof(original));

            var type = original.DeclaringType;
            var typeSystem = type.Module.TypeSystem;
            var ulongType = context.Use(typeSystem.UInt64);
            var signature = context.Use(uniqueSignatureType);
            
            var replacement = new MethodDefinition(".ctor", original.Attributes, original.ReturnType);
            foreach (var p in original.Parameters) {
                replacement.Parameters.Add(p);
            }
            
            var handle = new ParameterDefinition("createHandle", ParameterAttributes.None, ulongType);
            var dummy = new ParameterDefinition("dummy", ParameterAttributes.None, signature);

            replacement.Parameters.Add(handle);
            replacement.Parameters.Add(dummy);

            replacement.Body = original.Body;
            type.Methods.Add(replacement);

            return replacement;
        }

        public void Redirect(MethodDefinition original, MethodDefinition replacement, DatabaseTypeState state) {
            Guard.NotNull(original, nameof(original));
            Guard.NotNull(replacement, nameof(replacement));
            Guard.NotNull(state, nameof(state));

            original.Body.Instructions.Clear();
            var il = original.Body.GetILProcessor();

            il.Append(il.Create(OpCodes.Ldarg_0));
            foreach (var p in original.Parameters) {
                il.Append(il.Create(OpCodes.Ldarg, p));
            }
            il.Append(il.Create(OpCodes.Ldsfld, state.CreateHandle));
            il.Append(il.Create(OpCodes.Ldnull));
            il.Append(il.Create(OpCodes.Call, replacement));
            il.Append(il.Create(OpCodes.Ret));
        }
    }
}