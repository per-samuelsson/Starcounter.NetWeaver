
using Mono.Cecil;
using Starcounter.Hosting.Schema;
using System.Linq;

namespace starweave.Weaver {

    /// <summary>
    /// Define how identified database types (in the form of TypeDefinition and
    /// PropertyDefinition instances) bind to the schema produced by the analyzer,
    /// and some helper methods to do lookups not violating that.
    /// </summary>
    public static class TypeDefintitionBindingExtensions {
        const string BindingNameOfNoDeclaredBaseType = null;

        public static string GetBindingName(this TypeReference typeRef) {
            return typeRef.FullName;
        }

        public static string GetBindingName(this PropertyReference property) {
            return property.Name;
        }

        public static string GetBaseTypeBindingName(this TypeDefinition typeDef) {
            var typeSystem = typeDef.Module.TypeSystem;
            var useDeclaredBaseType = typeDef.BaseType != null && !typeDef.BaseType.Equals(typeSystem.Object);
            return useDeclaredBaseType ? typeDef.BaseType.GetBindingName() : BindingNameOfNoDeclaredBaseType; 
        }

        public static DatabaseType GetDatabaseType(this TypeReference typeRef, DatabaseSchema schema) {
            return schema.FindDatabaseType(typeRef.GetBindingName());
        }

        public static DatabaseType GetDatabaseType(this TypeReference typeRef, DatabaseAssembly assembly) {
            return assembly.FindType(typeRef.GetBindingName());
        }

        public static DatabaseProperty GetDatabaseProperty(this DatabaseType type, PropertyReference property) {
            return type.Properties.SingleOrDefault(p => p.Name.Equals(property.GetBindingName()));
        }

        public static PropertyDefinition GetPropertyDefinition(this DatabaseProperty property, TypeDefinition type) {
            return type.Properties.SingleOrDefault(p => p.GetBindingName().Equals(property.Name));
        }

        public static TypeDefinition GetTypeDefinition(this DatabaseType databaseType, ModuleDefinition module) {
            // Let's use SingleOrDefault as a safety assumption, even though we
            // probably will never see it fail even using (faster) First().
            return module.Types.SingleOrDefault(td => td.GetBindingName().Equals(databaseType.FullName));
        }
    }
}