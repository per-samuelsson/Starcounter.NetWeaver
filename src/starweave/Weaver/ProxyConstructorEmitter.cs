﻿
using Mono.Cecil;
using Mono.Cecil.Cil;
using Starcounter.Weaver;
using System;

namespace starweave.Weaver {

    public sealed class ProxyConstructorEmitter : ProxyConstructorFinder {
        readonly CodeEmissionContext context;

        // public [ctor]([SignatureType] t) <: base(t)> {
        // }

        public ProxyConstructorEmitter(CodeEmissionContext emitContext, TypeReference signatureTypeRef) : base(signatureTypeRef) {
            context = emitContext ?? throw new ArgumentNullException(nameof(emitContext));
        }

        public MethodDefinition Emit(TypeDefinition type) {
            var ctor = EmitMethodDefinition(type);

            var objectConstructor = context.Use(type.Module.GetObjectConstructorReference());
            ctor.Body.MaxStackSize = 8;
            var il = ctor.Body.GetILProcessor();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, objectConstructor);
            il.Emit(OpCodes.Ret);

            return ctor;
        }

        public MethodDefinition Emit(TypeDefinition type, MethodReference baseConstructor) {
            if (baseConstructor == null) {
                throw new ArgumentNullException(nameof(baseConstructor));
            }
            var baseCtor = context.Use(baseConstructor);

            var ctor = EmitMethodDefinition(type);
            ctor.Body.MaxStackSize = 8;
            var il = ctor.Body.GetILProcessor();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Call, baseCtor);
            il.Emit(OpCodes.Ret);

            return ctor;
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
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName,
                voidType);
            var dummy = new ParameterDefinition("dummy", ParameterAttributes.None, signature);

            ctor.Parameters.Add(dummy);
            type.Methods.Add(ctor);

            return ctor;
        }
    }
}