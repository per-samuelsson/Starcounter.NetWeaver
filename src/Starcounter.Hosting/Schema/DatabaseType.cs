
using System;
using System.Collections.Generic;

namespace Starcounter.Hosting.Schema {

    public class DatabaseType : IDataType {
        readonly int nameHandle;
        readonly int? baseNameHandle;
        readonly Dictionary<string, DatabaseProperty> properties = new Dictionary<string, DatabaseProperty>();

        public DatabaseAssembly DefiningAssembly {
            get;
            internal set;
        }

        public string FullName {
            get {
                return DefiningAssembly.DefiningSchema.TypeSystem.GetTypeNameByHandle(nameHandle);
            }
        }

        public string BaseTypeName {
            get {
                return baseNameHandle != null ?
                    DefiningAssembly.DefiningSchema.TypeSystem.GetTypeNameByHandle(nameHandle) : 
                    null;
            }
        }

        public IEnumerable<DatabaseProperty> Properties {
            get {
                return properties.Values;
            }
        }

        string IDataType.Name => FullName;

        // Deserialization support
        private DatabaseType() { }

        public DatabaseType(DatabaseAssembly definingAssembly, int typeHandle, int? baseTypeHandle) {
            DefiningAssembly = definingAssembly;
            nameHandle = typeHandle;
            baseNameHandle = baseTypeHandle;
        }

        public DatabaseType GetBaseType() {
            return DefiningAssembly.DefiningSchema.FindDatabaseType(BaseTypeName);
        }

        public bool IsDefinedIn(DatabaseAssembly assembly) {
            if (assembly == null) {
                throw new ArgumentNullException(nameof(assembly));
            }

            return assembly.Equals(DefiningAssembly);
        }
        
        public DatabaseProperty DefineProperty(string name, string dataType) {
            if (string.IsNullOrWhiteSpace(name)) {
                throw new ArgumentNullException(nameof(name));
            }
            if (string.IsNullOrWhiteSpace(dataType)) {
                throw new ArgumentNullException(nameof(dataType));
            }

            bool ignored;
            var typeSystem = DefiningAssembly.DefiningSchema.TypeSystem;
            var dataTypeHandle = typeSystem.GetTypeHandleByName(dataType, out ignored);

            var property = new DatabaseProperty(this, name, dataTypeHandle);
            properties.Add(name, property);

            return property;
        }
    }
}