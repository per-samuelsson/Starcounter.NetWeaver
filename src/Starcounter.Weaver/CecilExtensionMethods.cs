using Mono.Cecil;
using System;
using System.Linq;

namespace Starcounter.Weaver {

    public static class CecilExtensionMethods {

        public static bool IsAutoImplemented(this PropertyDefinition p) {
            var type = p.DeclaringType;
            var name = $"<{p.Name}>k__BackingField";
            return type.HasFields && type.Fields.Any(f => f.Name == name);
        }

        public static bool HasCustomAttribute(this TypeDefinition type, Type attributeType, bool declaredOnly = false) {
            return type.HasCustomAttribute(attributeType.FullName, declaredOnly);
        }

        public static bool HasCustomAttribute(this TypeDefinition type, string fullName, bool declaredOnly = false) {
            var hasDatabaseAttribute = type.HasCustomAttributes && type.CustomAttributes.Any(ca => ca.AttributeType.FullName == fullName);
            if (!hasDatabaseAttribute && !declaredOnly) {
                var baseDefinition = type.BaseType?.Resolve();
                if (baseDefinition != null) {
                    return HasCustomAttribute(baseDefinition, fullName, declaredOnly);
                }
            }
            return hasDatabaseAttribute;
        }
    }
}
