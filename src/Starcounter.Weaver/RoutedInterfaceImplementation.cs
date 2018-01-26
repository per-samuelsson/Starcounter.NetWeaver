
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
        readonly TypeDefinition interfaceDefinition;
        readonly InterfaceImplementation interfaceImplementation;
        readonly TypeDefinition passThroughType;
        readonly Dictionary<MethodDefinition, RoutedMethodImplementation> methodRoutes = new Dictionary<MethodDefinition, RoutedMethodImplementation>();
        readonly Dictionary<PropertyDefinition, RoutedPropertyImplementation> propertyRoutes = new Dictionary<PropertyDefinition, RoutedPropertyImplementation>();

        public TypeReference InterfaceType {
            get {
                return interfaceDefinition;
            }
        }

        /// <summary>
        /// Provides the definite list of interfaces that need to be implemented to
        /// fully implement <paramref name="interfaceTypeDefinition"/>, ordered in a way
        /// that more generic interfaces are assured to be listed before specific ones.
        /// </summary>
        /// <param name="emissionContext"></param>
        /// <param name="interfaceTypeDefinition"></param>
        /// <param name="passThroughTypeDefinition"></param>
        /// <param name="routingTargetType"></param>
        /// <returns></returns>
        public static IEnumerable<RoutedInterfaceImplementation> NewImplementationStrategy(CodeEmissionContext emissionContext, TypeDefinition interfaceTypeDefinition, TypeDefinition passThroughTypeDefinition, TypeDefinition routingTargetType) {
            
            var interfaces = interfaceTypeDefinition.GetAllInterfaces().ToList();
            interfaces.Sort(new TypeSpecializationComparer());
            interfaces.Add(interfaceTypeDefinition);

            return interfaces.Select(
                spec => new RoutedInterfaceImplementation(
                    emissionContext,
                    spec,
                    passThroughTypeDefinition,
                    routingTargetType)
            );
        }
        
        public RoutedInterfaceImplementation(CodeEmissionContext emissionContext, TypeDefinition interfaceTypeDefinition, TypeDefinition passThroughTypeDefinition, TypeDefinition routingTargetType) {
            Guard.NotNull(emissionContext, nameof(emissionContext));
            Guard.NotNull(interfaceTypeDefinition, nameof(interfaceTypeDefinition));
            Guard.NotNull(passThroughTypeDefinition, nameof(passThroughTypeDefinition));
            Guard.NotNull(routingTargetType, nameof(routingTargetType));

            if (!interfaceTypeDefinition.IsInterface) {
                throw new ArgumentException($"Type {interfaceTypeDefinition.FullName} is not an interface", nameof(interfaceTypeDefinition));
            }

            interfaceDefinition = interfaceTypeDefinition;
            interfaceImplementation = new InterfaceImplementation(interfaceTypeDefinition);
            passThroughType = passThroughTypeDefinition;

            foreach (var interfaceMethod in interfaceTypeDefinition.Methods) {
                var target = routingTargetType.Methods.FirstOrDefault(method => IsQualifiedRoutingTarget(method, interfaceMethod, passThroughTypeDefinition));
                if (target == null) {
                    throw new ArgumentException($"Routing target type missing qualifying interface method {interfaceMethod.FullName}", nameof(routingTargetType));
                }
                
                methodRoutes.Add(interfaceMethod, new RoutedMethodImplementation(interfaceImplementation, interfaceMethod, target));
            }
            
            foreach (var p in interfaceTypeDefinition.Properties) {
                propertyRoutes.Add(p, new RoutedPropertyImplementation(interfaceImplementation, p));
            }
        }

        public RoutedInterfaceImplementation(CodeEmissionContext emissionContext, Type interfaceType, Type passThroughType, Type routingTargetType) :
            this(emissionContext, UseType(emissionContext, interfaceType), UseType(emissionContext, passThroughType), UseType(emissionContext, routingTargetType)) {
        }

        static TypeDefinition UseType(CodeEmissionContext emissionContext, Type type) {
            Guard.NotNull(emissionContext, nameof(emissionContext));
            Guard.NotNull(type, nameof(type));

            var typeRef = emissionContext.Use(type);
            return typeRef.Resolve();
        }
        
        public void ImplementOn(TypeDefinition type) {
            if (!passThroughType.IsAssignableFrom(type)) {
                throw new ArgumentException(
                    $"Type {passThroughType.FullName} must be assignable from type {type.FullName}", nameof(type));
            }

            if (type.ImplementInterface(interfaceImplementation.InterfaceType)) {
                throw new ArgumentException(
                    $"Type {type.FullName} already implement interface {interfaceDefinition.FullName}.", nameof(type));
            }

            var notImplementedInterface = interfaceDefinition.GetAllInterfaces().FirstOrDefault(i => !type.ImplementInterface(i));
            if (notImplementedInterface != null) {
                throw new ArgumentException(
                    $"Type {type.FullName} must implement all base interfaces of {interfaceDefinition.FullName}. Interface {notImplementedInterface.FullName} is not implemented.", nameof(type));
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

        public static bool IsQualifiedRoutingTarget(MethodDefinition target, MethodDefinition interfaceMethod, TypeDefinition passThrough) {
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

                if (!x.ParameterType.ReferenceSameType(y.ParameterType)) {
                    return false;
                }
            }

            return true;
        }
    }
}
