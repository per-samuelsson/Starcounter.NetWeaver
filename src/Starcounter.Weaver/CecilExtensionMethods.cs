
using Mono.Cecil;
using System;
using System.Linq;

namespace Starcounter.Weaver {

    public static class CecilExtensionMethods {

        public static bool IsAutoImplemented(this PropertyDefinition p) {
            return AutoImplementedProperty.IsAutoImplementedProperty(p);
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

        public static MethodReference GetObjectConstructorReference(this ModuleDefinition module) {
            TypeReference tr;
            module.TryGetTypeReference(typeof(object).FullName, out tr);
            
            var type = tr.Resolve();
            return type.Methods.Single(m => m.Name.Equals(".ctor"));
        }

        public static byte[] ReadEmbeddedResource(this ModuleDefinition module, string name) {
            if (!module.HasResources) {
                return null;
            }

            var resource = module.Resources.FirstOrDefault((r) => {
                return r.ResourceType == ResourceType.Embedded && r.Name == name;
            }) as EmbeddedResource;

            return resource?.GetResourceData();
        }
    }
}
