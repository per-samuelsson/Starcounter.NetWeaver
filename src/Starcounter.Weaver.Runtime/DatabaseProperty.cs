
using Starcounter.Weaver.Runtime.Abstractions;

namespace Starcounter.Weaver.Runtime {

    public class DatabaseProperty {
        readonly string name;
        readonly int dataType;

        // Deserialization support
        private DatabaseProperty() { }

        internal DatabaseProperty(DatabaseType declaringType, string propertyName, int dataTypeHandle) {
            DeclaringType = declaringType;
            name = propertyName;
            dataType = dataTypeHandle;
        }

        public DatabaseType DeclaringType {
            get;
            internal set;
        }

        public string Name {
            get {
                return name;
            }
        }

        public IDataType DataType {
            get {
                return DeclaringType.DefiningAssembly.DefiningSchema.GetTypeByHandle(dataType);
            }
        }
    }
}