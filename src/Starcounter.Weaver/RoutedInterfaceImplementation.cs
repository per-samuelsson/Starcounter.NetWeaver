
using System;
using System.Collections.Generic;
using Mono.Cecil;
using System.Linq;
using Mono.Cecil.Cil;

namespace Starcounter.Weaver {

    /// <summary>
    /// Provide the service to implement a given interface on types, routing each
    /// method in that interface to a corresponding static method of a given target type,
    /// passing the instance as a first parameter.
    /// </summary>
    public class RoutedInterfaceImplementation {
        readonly InterfaceImplementation interfaceImplementation;
        readonly TypeDefinition passThroughInterface;
        readonly Dictionary<MethodDefinition, MethodDefinition> routes = new Dictionary<MethodDefinition, MethodDefinition>();

        public RoutedInterfaceImplementation(CodeEmissionContext emissionContext, TypeDefinition interfaceDefinition, TypeDefinition passThroughType, TypeDefinition routingTargetType) {
            Guard.NotNull(emissionContext, nameof(emissionContext));
            Guard.NotNull(interfaceDefinition, nameof(interfaceDefinition));
            Guard.NotNull(passThroughType, nameof(passThroughType));
            Guard.NotNull(routingTargetType, nameof(routingTargetType));

            if (!interfaceDefinition.IsInterface) {
                throw new ArgumentException($"Type {interfaceDefinition.FullName} is not an interface", nameof(interfaceDefinition));
            }

            if (!passThroughType.IsInterface) {
                throw new ArgumentException($"Type {passThroughType.FullName} is not an interface", nameof(passThroughType));
            }
            
            foreach (var m in interfaceDefinition.Methods) {
                var target = routingTargetType.Methods.FirstOrDefault(method => IsQualifiedRoutingTarget(method, m, passThroughType));
                if (target == null) {
                    throw new ArgumentException($"Routing target type missing qualifying interface method {m.FullName}", nameof(routingTargetType));
                }

                routes.Add(m, target);
            }

            interfaceImplementation = new InterfaceImplementation(interfaceDefinition);
            passThroughInterface = passThroughType;
        }
        
        public void ImplementOn(TypeDefinition type) {
            if (!type.ImplementInterface(passThroughInterface)) {
                throw new ArgumentException(
                    $"Type {type.FullName} does not implement pass-through interface {passThroughInterface.FullName}.", nameof(type));
            }

            type.Interfaces.Add(interfaceImplementation);
            foreach (var route in routes) {
                var name = interfaceImplementation.InterfaceType.FullName + "." + route.Key.Name;
                var attributes = route.Key.Attributes;
                attributes ^= MethodAttributes.Abstract;
                attributes ^= MethodAttributes.Public;
                attributes |= (MethodAttributes.Virtual | MethodAttributes.Private | MethodAttributes.HideBySig | MethodAttributes.Final);
                var m = new MethodDefinition(name, attributes, route.Key.ReturnType);
                m.Overrides.Add(route.Key);
                m.Body = new MethodBody(m);
                var il = m.Body.GetILProcessor();
                il.Emit(OpCodes.Ldarg_0);
                foreach (var p in route.Key.Parameters) {
                    il.Emit(OpCodes.Ldarg, p);
                }
                il.Emit(OpCodes.Call, route.Value);
                il.Emit(OpCodes.Ret);

                type.Methods.Add(m);
            }
        }

        static bool IsQualifiedRoutingTarget(MethodDefinition target, MethodDefinition interfaceMethod, TypeDefinition passThrough) {
            if (!target.IsStatic || target.Name != interfaceMethod.Name || !target.HasParameters) {
                return false;
            }

            var interfaceParameterCount = interfaceMethod.HasParameters ? interfaceMethod.Parameters.Count() : 0;
            if (target.Parameters.Count() != (interfaceParameterCount + 1)) {
                return false;
            }

            var passThroughParameter = target.Parameters[0];
            if ((passThroughParameter.Attributes & (ParameterAttributes.Out | ParameterAttributes.Retval)) != 0) {
                return false;
            }
            if (passThroughParameter.ParameterType.FullName != passThrough.FullName) {
                return false;
            }

            for (int i = 0; i < interfaceParameterCount; i++) {
                var x = interfaceMethod.Parameters[i];
                var y = target.Parameters[i + 1];

                if (x.Attributes != y.Attributes) {
                    return false;
                }

                if (!x.ParameterType.Equals(y.ParameterType)) {
                    return false;
                }
            }

            return true;
        }
    }
}
