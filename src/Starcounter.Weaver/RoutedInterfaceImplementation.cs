
using System;
using System.Collections.Generic;
using Mono.Cecil;
using System.Linq;

namespace Starcounter.Weaver {

    /// <summary>
    /// Provide the service to implement a given interface on types, routing each
    /// method in that interface to a corresponding static method of a given target type,
    /// passing the instance as a first parameter.
    /// </summary>
    public class RoutedInterfaceImplementation {
        readonly InterfaceImplementation interfaceImplementation;
        readonly TypeDefinition passThroughInterface;
        readonly Dictionary<MethodDefinition, RoutedMethodImplementation> methodRoutes = new Dictionary<MethodDefinition, RoutedMethodImplementation>();
        readonly Dictionary<PropertyDefinition, RoutedPropertyImplementation> propertyRoutes = new Dictionary<PropertyDefinition, RoutedPropertyImplementation>();

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

            interfaceImplementation = new InterfaceImplementation(interfaceDefinition);
            passThroughInterface = passThroughType;

            foreach (var m in interfaceDefinition.Methods) {
                var target = routingTargetType.Methods.FirstOrDefault(method => IsQualifiedRoutingTarget(method, m, passThroughType));
                if (target == null) {
                    throw new ArgumentException($"Routing target type missing qualifying interface method {m.FullName}", nameof(routingTargetType));
                }
                
                methodRoutes.Add(m, new RoutedMethodImplementation(interfaceImplementation, m, target));
            }
            
            foreach (var p in interfaceDefinition.Properties) {
                propertyRoutes.Add(p, new RoutedPropertyImplementation(interfaceImplementation, p));
            }
        }
        
        public void ImplementOn(TypeDefinition type) {
            if (!type.ImplementInterface(passThroughInterface)) {
                throw new ArgumentException(
                    $"Type {type.FullName} does not implement pass-through interface {passThroughInterface.FullName}.", nameof(type));
            }

            type.Interfaces.Add(interfaceImplementation);
            
            foreach (var route in methodRoutes.Values) {
                route.ImplementOn(type);
            }

            foreach (var p in propertyRoutes) {
                var interfaceProperty = p.Key;
                var route = p.Value;

                var getter = methodRoutes[interfaceProperty.GetMethod];
                var setter = interfaceProperty.SetMethod != null ? methodRoutes[interfaceProperty.SetMethod] : null;

                route.ImplementOn(type, getter, setter);
            }
        }

        static bool IsQualifiedRoutingTarget(MethodDefinition target, MethodDefinition interfaceMethod, TypeDefinition passThrough) {
            if (!target.IsStatic || !target.HasParameters) {
                return false;
            }
            
            // To support properties, we give target methods the option to name like Get_Foo and Set_Foo,
            // instead of get_Foo/set_Foo, allowing code that would raise warnings/errors by code-style
            // analyzers.

            var comparison = interfaceMethod.IsGetter || interfaceMethod.IsSetter ?
                StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture;

            if (!interfaceMethod.Name.Equals(target.Name, comparison)) {
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
