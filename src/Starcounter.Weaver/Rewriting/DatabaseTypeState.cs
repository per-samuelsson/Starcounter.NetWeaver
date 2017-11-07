
using Mono.Cecil;
using System.Linq;

namespace Starcounter.Weaver.Rewriting {

    public class DatabaseTypeState {
        protected readonly TypeDefinition type;
        protected readonly DatabaseTypeStateNames stateNames;

        public FieldReference DbId {
            get {
                return GetFieldRecursive(type, stateNames.DbId);
            }
        }

        public FieldReference DbRef {
            get {
                return GetFieldRecursive(type, stateNames.DbRef);
            }
        }

        static FieldReference GetFieldRecursive(TypeDefinition t, string name) {
            var result = t.Fields.SingleOrDefault(f => f.Name.Equals(name));
            if (result == null && t.BaseType != null) {
                t = t.BaseType.Resolve();
                return GetFieldRecursive(t, name);
            }
            return result;
        }

        public FieldDefinition CreateHandle {
            get {
                return type.Fields.Single(f => f.Name.Equals(stateNames.CreateHandle));
            }
        }

        public FieldDefinition DeleteHandle {
            get {
                return type.Fields.Single(f => f.Name.Equals(stateNames.DeleteHandle));
            }
        }

        public FieldDefinition GetPropertyHandle(PropertyDefinition property) {
            return GetPropertyHandle(property.Name);
        }

        public FieldDefinition GetPropertyHandle(string propertyName) {
            return type.Fields.Single(f => f.Name.Equals(stateNames.GetPropertyHandleName(propertyName)));
        }

        public DatabaseTypeState(TypeDefinition typeDefinition, DatabaseTypeStateNames names) {
            Guard.NotNull(typeDefinition, nameof(typeDefinition));
            Guard.NotNull(names, nameof(names));
            type = typeDefinition;
            stateNames = names;
        }
    }
}