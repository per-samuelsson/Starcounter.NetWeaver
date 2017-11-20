
using Mono.Cecil;
using Starcounter.Weaver;
using System;

namespace starweave.Weaver {

    public class DatabaseTypeStateEmitter : DatabaseTypeState {
        readonly TypeReference ulongType;
        readonly CodeEmissionContext context;

        public DatabaseTypeStateEmitter(CodeEmissionContext emitContext, TypeDefinition typeDefinition, DatabaseTypeStateNames names) : base(typeDefinition, names) {
            context = emitContext ?? throw new ArgumentNullException(nameof(emitContext));
            ulongType = context.Use(type.Module.TypeSystem.UInt64);
        }

        public void EmitReferenceFields() {
            var f = new FieldDefinition(stateNames.DbId, FieldAttributes.Family, ulongType);
            type.Fields.Add(f);
            f = new FieldDefinition(stateNames.DbRef, FieldAttributes.Family, ulongType);
            type.Fields.Add(f);
        }

        public void EmitCRUDHandles() {
            var f = new FieldDefinition(stateNames.CreateHandle, FieldAttributes.Static, ulongType);
            type.Fields.Add(f);
            f = new FieldDefinition(stateNames.DeleteHandle, FieldAttributes.Static, ulongType);
            type.Fields.Add(f);
        }

        public void EmitPropertyCRUDHandle(string propertyName) {
            var name = stateNames.GetPropertyHandleName(propertyName);
            var f = new FieldDefinition(name, FieldAttributes.Static, ulongType);
            type.Fields.Add(f);
        }
    }
}