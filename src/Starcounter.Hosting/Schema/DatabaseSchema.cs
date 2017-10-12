
using System;
using System.Collections.Generic;

namespace Starcounter.Hosting.Schema {

    public sealed class DatabaseSchema {
        Dictionary<string, DatabaseAssembly> assemblies = new Dictionary<string, DatabaseAssembly>();

        public IEnumerable<DatabaseAssembly> Assemblies {
            get {
                return assemblies.Values;
            }
        }

        public DatabaseAssembly DefineAssembly(string name) {
            if (string.IsNullOrWhiteSpace(name)) {
                throw new ArgumentNullException(nameof(name));
            }

            var assembly = new DatabaseAssembly();
            assembly.Name = name;
            assembly.DefiningSchema = this;
            assemblies.Add(assembly.Name, assembly);

            return assembly;
        }

        internal DatabaseType FindType(string name) {
            if (string.IsNullOrWhiteSpace(name)) {
                throw new ArgumentNullException(nameof(name));
            }

            // Could consider indexing all types in the schema by name
            // since we control them being added, but let's keep it simple
            // to begin with.

            foreach (var assembly in assemblies.Values) {
                var t = assembly.FindType(name);
                if (t != null) {
                    return t;
                }
            }

            return null;
        }
    }
}