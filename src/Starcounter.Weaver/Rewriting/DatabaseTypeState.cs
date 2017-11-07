
using Mono.Cecil;
using System.Linq;

namespace Starcounter.Weaver.Rewriting {

    public class DatabaseTypeState {
        protected readonly TypeDefinition type;
        protected readonly DatabaseTypeStateNames stateNames;

        public FieldDefinition DbId {
            get {
                return type.Fields.Single(f => f.Name.Equals(stateNames.DbId));
            }
        }

        public FieldDefinition DbRef {
            get {
                return type.Fields.Single(f => f.Name.Equals(stateNames.DbRef));
            }
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
        }
    }
}