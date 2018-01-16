
using System;
using System.Collections.Generic;
using System.Linq;

namespace Starcounter.Weaver.Runtime {

    public class DatabaseAssembly {
        Dictionary<string, DatabaseType> types = new Dictionary<string, DatabaseType>();

        public DatabaseSchema DefiningSchema { get; set; }

        public string Name { get; set; }

        public IEnumerable<DatabaseType> Types {
            get {
                return types.Values;
            }
        }

        // Deserialization support
        private DatabaseAssembly() {}

        public DatabaseAssembly(DatabaseSchema schema, string name) {
            DefiningSchema = schema;
            Name = name;
        }
        
        public IEnumerable<DatabaseType> DefineTypes(Tuple<string, string>[] nameAndBaseNames) {
            if (nameAndBaseNames == null) {
                throw new ArgumentNullException(nameof(nameAndBaseNames));
            }
            
            bool CanResolveBaseTypeName(string name, DatabaseSchema schema, Tuple<string, string>[] registrars) {
                if (name == null) {
                    return true;
                }

                if (registrars.Any(t => t.Item1.Equals(name))) {
                    return true;
                }

                return schema.FindDatabaseType(name) != null;
            }
            
            var typeSystem = DefiningSchema.TypeSystem;
            
            foreach (var typeDefinition in nameAndBaseNames) {
                var typeName = typeDefinition.Item1;
                var baseName = typeDefinition.Item2;

                if (string.IsNullOrWhiteSpace(typeName)) {
                    throw new ArgumentNullException("All types must define a name");
                }

                if (!CanResolveBaseTypeName(baseName, DefiningSchema, nameAndBaseNames)) {
                    var msg = $"Type {typeName} define base type {baseName} which is not defined";
                    throw new ArgumentOutOfRangeException(nameof(nameAndBaseNames), msg);
                }

                if (typeSystem.ContainsType(typeName)) {
                    throw new InvalidOperationException($"A type named {typeName} is already defined");
                }
            }
            
            foreach (var typeDefinition in nameAndBaseNames) {
                var typeName = typeDefinition.Item1;
                typeSystem.DefineDatabaseType(typeName);
            }
            
            foreach (var typeDefinition in nameAndBaseNames) {
                var typeName = typeDefinition.Item1;
                var baseName = typeDefinition.Item2;

                bool ignored;
                int? baseHandle = null;

                if (baseName != null) {
                    baseHandle = typeSystem.GetTypeHandleByName(baseName, out ignored);
                }
                var handle = typeSystem.GetTypeHandleByName(typeName, out ignored);

                var type = new DatabaseType(this, handle, baseHandle);
                types.Add(type.FullName, type);
            }

            return types.Values;
        }

        public DatabaseType FindType(string name) {
            if (string.IsNullOrWhiteSpace(name)) {
                throw new ArgumentNullException(nameof(name));
            }

            return types.TryGetValue(name, out DatabaseType type) ? type : null;
        }
    }
}