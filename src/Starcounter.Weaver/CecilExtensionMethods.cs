
using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Starcounter.Weaver {

    public static class CecilExtensionMethods {

        public static bool ReferenceSameMethod(this MethodReference reference, MethodReference other) {
            // Meta tokens aren't accurate: definition will have a different token than
            // any reference. FullName is slow but at least functional here.
            return reference.FullName.Equals(other.FullName);
        }

        public static bool ReferenceSameType(this TypeReference reference, TypeReference other) {
            // Meta tokens aren't accurate: definition will have a different token than
            // any reference. FullName is slow but at least functional here.
            return reference.FullName.Equals(other.FullName);
        }

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

        public static FieldReference GetFieldRecursive(this TypeDefinition type, string name) {
            var result = type.Fields.SingleOrDefault(f => f.Name.Equals(name));
            if (result == null && type.BaseType != null) {
                type = type.BaseType.Resolve();
                return GetFieldRecursive(type, name);
            }
            return result;
        }

        public static IEnumerable<MethodDefinition> GetInstanceConstructors(this TypeDefinition type) {
            return type.Methods.Where(m => m.IsConstructor && m.HasThis);
        }

        public static MethodReference GetObjectConstructorReference(this ModuleDefinition module) {
            module.TryGetTypeReference(typeof(object).FullName, out TypeReference tr);

            var type = tr.Resolve();
            return type.Methods.Single(m => m.Name.Equals(".ctor"));
        }

        public static MethodDefinition Clone(this MethodDefinition method, string newName = null) {
            // Not sure this one is sufficient. We need some way to do this though,
            // so let's start here.
            // Then keep an eye on: https://github.com/jbevain/cecil/issues/476
            var clone = new MethodDefinition(newName ?? method.Name, method.Attributes, method.ReturnType);
            foreach (var p in method.Parameters) {
                clone.Parameters.Add(p);
            }

            clone.Body = new MethodBody(clone);
            clone.Body.MaxStackSize = method.Body.MaxStackSize;
            foreach (var instruction in method.Body.Instructions) {
                clone.Body.Instructions.Add(instruction);
            }

            return clone;
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
