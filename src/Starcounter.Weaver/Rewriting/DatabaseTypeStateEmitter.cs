
using Mono.Cecil;

namespace Starcounter.Weaver.Rewriting {

    public class DatabaseTypeStateEmitter : DatabaseTypeState {
        readonly TypeReference ulongType;

        public DatabaseTypeStateEmitter(TypeDefinition typeDefinition) : base(typeDefinition) {
            ulongType = type.Module.ImportReference(typeof(ulong));
        }

        public void EmitReferenceFields() {
            var f = new FieldDefinition(DatabaseTypeStateNames.DbId, FieldAttributes.Family, ulongType);
            type.Fields.Add(f);
            f = new FieldDefinition(DatabaseTypeStateNames.DbRef, FieldAttributes.Family, ulongType);
            type.Fields.Add(f);
        }

        public void EmitCRUDHandles() {
            var f = new FieldDefinition(DatabaseTypeStateNames.CreateHandle, FieldAttributes.Static, ulongType);
            type.Fields.Add(f);
            f = new FieldDefinition(DatabaseTypeStateNames.DeleteHandle, FieldAttributes.Static, ulongType);
            type.Fields.Add(f);
        }

        public void EmitPropertyCRUDHandle(string propertyName) {
            var name = DatabaseTypeStateNames.GetPropertyHandleName(propertyName);
            var f = new FieldDefinition(name, FieldAttributes.Static, ulongType);
            type.Fields.Add(f);
        }
    }
}