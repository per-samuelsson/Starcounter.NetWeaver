
using System;
using System.Collections.Generic;
using System.Linq;

namespace Starcounter.Hosting.Schema {

    public class DatabaseAssembly {
        Dictionary<string, DatabaseType> types = new Dictionary<string, DatabaseType>();

        public DatabaseSchema DefiningSchema { get; internal set; }

        public string Name { get; set; }

        public IEnumerable<DatabaseType> Types {
            get {
                return types.Values;
            }
        }
        
        public IEnumerable<DatabaseType> DefineTypes(Tuple<string, string>[] nameAndBaseNames) {
            if (nameAndBaseNames == null) {
                throw new ArgumentNullException(nameof(nameAndBaseNames));
            }

            bool ResolveBaseTypeName(string name, DatabaseSchema schema, Tuple<string, string>[] registrars) {
                if (registrars.Any(t => t.Item1.Equals(name))) {
                    return true;
                }

                return schema.FindType(name) != null;
            }

            var additions = new List<DatabaseType>(nameAndBaseNames.Length);

            foreach (var typeDefinition in nameAndBaseNames) {
                if (!ResolveBaseTypeName(typeDefinition.Item2, DefiningSchema, nameAndBaseNames)) {
                    var msg = $"Type {typeDefinition.Item1} define base type {typeDefinition.Item2} which is not defined";
                    throw new ArgumentOutOfRangeException(nameof(nameAndBaseNames), msg);
                }
                additions.Add(new DatabaseType() {
                    FullName = typeDefinition.Item1,
                    BaseTypeName = typeDefinition.Item2
                });
            }

            AddTypesSafe(additions);
            return additions;
        }

        void AddTypesSafe(IEnumerable<DatabaseType> databaseTypes) {
            foreach (var type in databaseTypes) {
                types.Add(type.FullName, type);
                type.DefiningAssembly = this;
            }
        }

        internal DatabaseType FindType(string name) {
            if (string.IsNullOrWhiteSpace(name)) {
                throw new ArgumentNullException(nameof(name));
            }

            DatabaseType type;
            return types.TryGetValue(name, out type) ? type : null;
        }
    }
}