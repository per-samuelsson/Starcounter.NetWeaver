
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using Starcounter.Weaver;
using System;
using System.Collections.Generic;

namespace starweave.Weaver {

    public class DatabaseTypeIDbProxyStateEmitter {
        const string GetIdName = "GetDbId";
        const string SetIdName = "SetDbId";
        const string GetRefName = "GetDbRef";
        const string SetRefName = "SetDbRef";

        readonly InterfaceImplementation implementation;
        readonly CodeEmissionContext context;

        readonly Dictionary<string, MethodDefinition> methods = new Dictionary<string, MethodDefinition>() {
            { GetIdName, null },
            { SetIdName, null },
            { GetRefName, null },
            { SetRefName, null }
        };

        public DatabaseTypeIDbProxyStateEmitter(CodeEmissionContext emitContext, TypeDefinition proxyStateInterface) {
            Guard.NotNull(emitContext, nameof(emitContext));
            Guard.NotNull(proxyStateInterface, nameof(proxyStateInterface));

            if (!proxyStateInterface.IsInterface) {
                throw new ArgumentException($"Type {proxyStateInterface.FullName} is not an interface", nameof(proxyStateInterface));
            }

            context = emitContext;

            var typeRef = context.Use(proxyStateInterface);
            implementation = new InterfaceImplementation(typeRef);
            
            foreach (var interfaceMethod in proxyStateInterface.Methods) {
                if (!methods.ContainsKey(interfaceMethod.Name)) {
                    throw new ArgumentException($"Interface {proxyStateInterface.FullName} expose unsupported method {interfaceMethod.Name}.");
                }
                methods[interfaceMethod.Name] = interfaceMethod;
            }
        }
        
        public DatabaseTypeIDbProxyStateEmitter(CodeEmissionContext emitContext, Type proxyStateInterface) : this(emitContext, UseDefinition(emitContext, proxyStateInterface)) {
        }

        static TypeDefinition UseDefinition(CodeEmissionContext emitContext, Type proxyStateInterface) {
            Guard.NotNull(emitContext, nameof(emitContext));
            Guard.NotNull(proxyStateInterface, nameof(proxyStateInterface));

            var typeRef = emitContext.Use(proxyStateInterface);
            return typeRef.Resolve();
        }

        public void ImplementOn(TypeDefinition type, DatabaseTypeState state) {
            var ulongType = context.Use(type.Module.TypeSystem.UInt64);
            var voidType = context.Use(type.Module.TypeSystem.Void);

            type.Interfaces.Add(implementation);

            var getId = methods[GetIdName];
            if (getId != null) {
                AssureCorrectGetterMethod(getId, ulongType);
                ImplementGetMethod(getId, type, state.DbId);
            }

            var setId = methods[SetIdName];
            if (setId != null) {
                AssureCorrectSetterMethod(setId, voidType, ulongType);
                ImplementSetMethod(setId, type, state.DbId);
            }

            var getRef = methods[GetRefName];
            if (getRef != null) {
                AssureCorrectGetterMethod(getRef, ulongType);
                ImplementGetMethod(getRef, type, state.DbRef);
            }

            var setRef = methods[SetRefName];
            if (setRef != null) {
                AssureCorrectSetterMethod(setRef, voidType, ulongType);
                ImplementSetMethod(setRef, type, state.DbRef);
            }
        }

        void AssureCorrectGetterMethod(MethodDefinition m, TypeReference expectedReturnType) {
            if (m.Parameters.Count != 0 || !m.ReturnType.ReferenceSameType(expectedReturnType)) {
                throw new InvalidOperationException($"Stored interface method {m.FullName} does not meet expected requirements of a getter method");
            }
        }

        void AssureCorrectSetterMethod(MethodDefinition m, TypeReference expectedReturnType, TypeReference expectedParameterType) {
            if (m.Parameters.Count != 1 || 
                !m.Parameters[0].ParameterType.ReferenceSameType(expectedParameterType) ||
                !m.ReturnType.ReferenceSameType(expectedReturnType)) {
                throw new InvalidOperationException($"Stored interface method {m.FullName} does not meet expected requirements of a setter method");
            }
        }

        void ImplementGetMethod(MethodDefinition getter, TypeDefinition type, FieldReference state) {
            var name = implementation.InterfaceType.FullName + "." + getter.Name;
            var attributes = getter.Attributes;
            attributes ^= MethodAttributes.Abstract;
            attributes ^= MethodAttributes.Public;
            attributes |= (MethodAttributes.Virtual | MethodAttributes.Private | MethodAttributes.HideBySig | MethodAttributes.Final);

            var m = new MethodDefinition(name, attributes, getter.ReturnType);
            m.Overrides.Add(getter);

            m.Body = new MethodBody(m);
            var il = m.Body.GetILProcessor();
            il.Append(il.Create(OpCodes.Ldarg_0));
            il.Append(il.Create(OpCodes.Ldfld, state));
            il.Append(il.Create(OpCodes.Ret));

            il.Body.OptimizeMacros();

            type.Methods.Add(m);
        }

        void ImplementSetMethod(MethodDefinition setter, TypeDefinition type, FieldReference state) {
            var name = implementation.InterfaceType.FullName + "." + setter.Name;
            var attributes = setter.Attributes;
            attributes ^= MethodAttributes.Abstract;
            attributes ^= MethodAttributes.Public;
            attributes |= (MethodAttributes.Virtual | MethodAttributes.Private | MethodAttributes.HideBySig | MethodAttributes.Final);

            var m = new MethodDefinition(name, attributes, setter.ReturnType);
            var longParam = setter.Parameters[0];
            m.Parameters.Add(new ParameterDefinition(
                longParam.Name, longParam.Attributes, longParam.ParameterType));
            m.Overrides.Add(setter);

            m.Body = new MethodBody(m);
            var il = m.Body.GetILProcessor();
            il.Append(il.Create(OpCodes.Ldarg_0));
            il.Append(il.Create(OpCodes.Ldarg_1));
            il.Append(il.Create(OpCodes.Stfld, state));
            il.Append(il.Create(OpCodes.Ret));

            il.Body.OptimizeMacros();

            type.Methods.Add(m);
        }
    }
}
