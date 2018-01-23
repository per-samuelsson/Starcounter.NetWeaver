
using Mono.Cecil;

namespace Starcounter.Weaver {

    public class RoutedPropertyImplementation {
        readonly InterfaceImplementation interfaceImplementation;
        readonly PropertyDefinition interfaceProperty;

        public PropertyDefinition ImplementedProperty {
            get;
            private set;
        }

        public RoutedPropertyImplementation(InterfaceImplementation implementation, PropertyDefinition property) {
            Guard.NotNull(implementation, nameof(implementation));
            Guard.NotNull(property, nameof(property));
            interfaceProperty = property;
            interfaceImplementation = implementation;
        }

        public void ImplementOn(
            TypeDefinition type,
            RoutedMethodImplementation getter,
            RoutedMethodImplementation setter = null) {
            Guard.NotNull(type, nameof(type));
            Guard.NotNull(getter, nameof(getter));

            var p = new PropertyDefinition(interfaceProperty.Name, interfaceProperty.Attributes, interfaceProperty.PropertyType) {
                GetMethod = getter.ImplementedMethod,
                SetMethod = setter?.ImplementedMethod
            };
            type.Properties.Add(p);

            ImplementedProperty = p;
        }
    }
}
