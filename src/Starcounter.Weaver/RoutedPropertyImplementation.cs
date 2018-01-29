
using Mono.Cecil;

namespace Starcounter.Weaver {

    public class RoutedPropertyImplementation {
        readonly CodeEmissionContext emitContext;
        readonly InterfaceImplementation interfaceImplementation;
        readonly PropertyDefinition interfaceProperty;
        readonly TypeReference interfacePropertyTypeRef;
        
        public RoutedPropertyImplementation(CodeEmissionContext codeEmissionContext, InterfaceImplementation implementation, PropertyDefinition property) {
            Guard.NotNull(codeEmissionContext, nameof(codeEmissionContext));
            Guard.NotNull(implementation, nameof(implementation));
            Guard.NotNull(property, nameof(property));

            emitContext = codeEmissionContext;
            interfaceProperty = property;
            interfaceImplementation = implementation;
            interfacePropertyTypeRef = emitContext.Use(property.PropertyType);
        }

        public void ImplementOn(
            TypeDefinition type,
            RoutedMethodImplementation getter,
            RoutedMethodImplementation setter = null) {
            Guard.NotNull(type, nameof(type));
            Guard.NotNull(getter, nameof(getter));

            var name = interfaceImplementation.InterfaceType.FullName + "." + interfaceProperty.Name;

            var p = new PropertyDefinition(name, interfaceProperty.Attributes, interfacePropertyTypeRef) {
                GetMethod = getter.ImplementedMethod,
                SetMethod = setter?.ImplementedMethod
            };
            type.Properties.Add(p);
        }
    }
}
