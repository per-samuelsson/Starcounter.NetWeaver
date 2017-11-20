
using Mono.Cecil;
using Mono.Cecil.Cil;
using Starcounter.Weaver;
using System;

namespace starweave.Weaver {

    public sealed class InsertConstructorEmitter {
        readonly TypeReference signatureType;
        readonly MethodReference insertMethod;
        readonly CodeEmissionContext context;
        
        // private [ctor](ulong handle, [SignatureType]) {
        //   [InsertMethod](out this.dbId, out this.dbRef, handle);
        // }

        public InsertConstructorEmitter(CodeEmissionContext emitContext, TypeReference signatureTypeRef, MethodReference insertMethodRef) {
            signatureType = signatureTypeRef ?? throw new ArgumentNullException(nameof(signatureTypeRef));
            insertMethod = insertMethodRef ?? throw new ArgumentNullException(nameof(insertMethodRef));
            context = emitContext ?? throw new ArgumentNullException(nameof(emitContext));
        }

        public void Emit(TypeDefinition type, DatabaseTypeState state) {
            if (type == null) {
                throw new ArgumentNullException(nameof(type));
            }
            if (state == null) {
                throw new ArgumentNullException(nameof(state));
            }

            var typeSystem = type.Module.TypeSystem;
            var voidType = context.Use(typeSystem.Void);
            var ulongType = context.Use(typeSystem.UInt64);
            var signature = context.Use(signatureType);
            var insert = context.Use(insertMethod);
            var objectConstructor = context.Use(type.Module.GetObjectConstructorReference());

            var ctor = new MethodDefinition(
                ".ctor", 
                MethodAttributes.Private | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, 
                voidType);
            var handle = new ParameterDefinition("createHandle", ParameterAttributes.None, ulongType);
            var dummy = new ParameterDefinition("dummy", ParameterAttributes.None, signature);

            ctor.Parameters.Add(handle);
            ctor.Parameters.Add(dummy);

            ctor.Body.MaxStackSize = 8;
            var il = ctor.Body.GetILProcessor();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, objectConstructor);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldflda, state.DbId);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldflda, state.DbRef);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Call, insert);
            il.Emit(OpCodes.Ret);

            type.Methods.Add(ctor);
        }
    }
}