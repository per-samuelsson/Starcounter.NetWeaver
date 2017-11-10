
using Mono.Cecil;
using System;
using System.Linq;

namespace Starcounter.Weaver {

    public sealed class AutoImplementedProperty {
        readonly PropertyDefinition definition;
        readonly FieldDefinition backingField;

        public PropertyDefinition Property {
            get {
                return definition;
            }
        }

        public FieldDefinition BackingField {
            get {
                return backingField;
            }
        }

        public AutoImplementedProperty(PropertyDefinition property) {
            Guard.NotNull(property, nameof(property));

            var t = property.DeclaringType;
            if (!t.HasFields) {
                throw new ArgumentException("Not an auto-implemented property: declaring type has no fields");
            }
            backingField = GetBackingField(t, property);
            if (backingField == null) {
                throw new ArgumentException($"Not an auto-implemented property: backing field '{GetBackingFieldName(property)}' not found");
            }

            definition = property;
        }

        public static bool IsAutoImplementedProperty(PropertyDefinition p) {
            var type = p.DeclaringType;
            return GetBackingField(type, p) != null;
        }

        static string GetBackingFieldName(PropertyDefinition p) {
            return $"<{p.Name}>k__BackingField";
        }

        static FieldDefinition GetBackingField(TypeDefinition type, PropertyDefinition p) {
            return type.Fields.SingleOrDefault(f => f.Name == GetBackingFieldName(p));
        }
    }
}
