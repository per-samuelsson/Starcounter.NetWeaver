
using System;
using System.Collections.Generic;
using System.Linq;

namespace Starcounter.Hosting.Schema {
    
    public sealed class DatabaseSchema {
        readonly Dictionary<string, DatabaseAssembly> assemblies = new Dictionary<string, DatabaseAssembly>();
        readonly TypeSystem typeSystem = new TypeSystem();

        class NamedType : IDataType {
            readonly string name;

            public string Name => name;

            public NamedType(string typeName) {
                name = typeName;
            }
        }
        
        public IEnumerable<DatabaseAssembly> Assemblies {
            get {
                return assemblies.Values;
            }
        }

        public IEnumerable<IDataType> Types {
            get {
                var dataTypes = typeSystem.DataTypes;
                var databaseTypeCount = assemblies.Values.Sum(a => a.Types.Count());
                var types = new List<IDataType>(dataTypes.Count() + databaseTypeCount);
                foreach (var name in dataTypes) {
                    types.Add(new NamedType(name));
                }
                foreach (var assembly in assemblies.Values) {
                    types.AddRange(assembly.Types);
                }
                return types;
            }
        }

        internal TypeSystem TypeSystem {
            get {
                return typeSystem;
            }
        }

        public void DefineDataType(string typeName) {
            typeSystem.DefineDataType(typeName);
        }
        
        public DatabaseAssembly DefineAssembly(string name) {
            if (string.IsNullOrWhiteSpace(name)) {
                throw new ArgumentNullException(nameof(name));
            }

            var assembly = new DatabaseAssembly(this, name);
            assemblies.Add(assembly.Name, assembly);

            return assembly;
        }

        public bool ContainSameAssemblies(DatabaseSchema other) {
            if (other == null) {
                throw new ArgumentNullException(nameof(other));
            }

            if (other.assemblies.Count != assemblies.Count) {
                return false;
            }

            foreach (var otherKey in other.assemblies.Keys) {
                if (!assemblies.ContainsKey(otherKey)) {
                    return false;
                }
            }

            return true;
        }

        public DatabaseType FindDatabaseType(string name) {
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
        
        internal IDataType GetTypeByHandle(int typeHandle) {
            bool isDataType;
            var name = typeSystem.GetTypeNameByHandle(typeHandle, out isDataType);
            return isDataType ? (IDataType) new NamedType(name) : FindDatabaseType(name);
        }
    }
}