
using Mono.Cecil;
using Mono.Cecil.Cil;
using Starcounter.Weaver;
using System;

namespace starweave.Weaver {
    
    public sealed class ProxyConstructorEmitter {
        readonly TypeReference signatureType;
        readonly CodeEmissionContext context;

        // public [ctor]([SignatureType] t) <: base(t)> {
        // }

        public ProxyConstructorEmitter(CodeEmissionContext emitContext, TypeReference signatureTypeRef) {
            signatureType = signatureTypeRef ?? throw new ArgumentNullException(nameof(signatureTypeRef));
            context = emitContext ?? throw new ArgumentNullException(nameof(emitContext));
        }

        public void Emit(TypeDefinition type) {
            var ctor = EmitMethodDefinition(type);

            var objectConstructor = context.Use(type.Module.GetObjectConstructorReference());
            ctor.Body.MaxStackSize = 8;
            var il = ctor.Body.GetILProcessor();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, objectConstructor);
            il.Emit(OpCodes.Ret);
        }

        public void Emit(TypeDefinition type, MethodReference baseConstructor) {
            if (baseConstructor == null) {
                throw new ArgumentNullException(nameof(baseConstructor));
            }

            var ctor = EmitMethodDefinition(type);
            ctor.Body.MaxStackSize = 8;
            var il = ctor.Body.GetILProcessor();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Call, baseConstructor);
            il.Emit(OpCodes.Ret);
        }

        MethodDefinition EmitMethodDefinition(TypeDefinition type) {
            if (type == null) {
                throw new ArgumentNullException(nameof(type));
            }

            var typeSystem = type.Module.TypeSystem;
            var voidType = context.Use(typeSystem.Void);
            var signature = context.Use(signatureType);

            var ctor = new MethodDefinition(
                ".ctor",
                MethodAttributes.Private | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName,
                voidType);
            var dummy = new ParameterDefinition("dummy", ParameterAttributes.None, signature);

            ctor.Parameters.Add(dummy);
            type.Methods.Add(ctor);

            return ctor;
        }
    }
}