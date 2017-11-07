
using Mono.Cecil;
using System.Linq;

namespace Starcounter.Weaver.Rewriting {

    public class DatabaseTypeState {
        protected readonly TypeDefinition type;

        public FieldDefinition DbId {
            get {
                return type.Fields.Single(f => f.Name.Equals(DatabaseTypeStateNames.DbId));
            }
        }

        public FieldDefinition DbRef {
            get {
                return type.Fields.Single(f => f.Name.Equals(DatabaseTypeStateNames.DbRef));
            }
        }

        public FieldDefinition CreateHandle {
            get {
                return type.Fields.Single(f => f.Name.Equals(DatabaseTypeStateNames.CreateHandle));
            }
        }

        public FieldDefinition DeleteHandle {
            get {
                return type.Fields.Single(f => f.Name.Equals(DatabaseTypeStateNames.DeleteHandle));
            }
        }

        public FieldDefinition GetPropertyHandle(PropertyDefinition property) {
            return GetPropertyHandle(property.Name);
        }

        public FieldDefinition GetPropertyHandle(string propertyName) {
            return type.Fields.Single(f => f.Name.Equals(DatabaseTypeStateNames.GetPropertyHandleName(propertyName)));
        }

        public DatabaseTypeState(TypeDefinition typeDefinition) {
            Guard.NotNull(typeDefinition, nameof(typeDefinition));
            type = typeDefinition;
        }
    }
}