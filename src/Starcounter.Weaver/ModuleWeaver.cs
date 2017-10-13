
using Mono.Cecil;
using Starcounter.Hosting.Schema;
using Starcounter.Weaver.Analysis;
using System.Linq;

namespace Starcounter.Weaver {
    
    public class ModuleWeaver {
        readonly SchemaSerializationContext schemaSerializationContext;

        public ModuleWeaver(SchemaSerializationContext serializationContext) {
            Guard.NotNull(serializationContext, nameof(serializationContext));
            schemaSerializationContext = serializationContext;
        }

        public ModuleDefinition Weave(ModuleDefinition module, DatabaseAssembly assembly) {
            Guard.NotNull(module, nameof(module));
            Guard.NotNull(assembly, nameof(assembly));

            // Run transformation first; serialize schema only when we know that has
            // succeeded.
            // TODO:

            schemaSerializationContext.Write(module, assembly.DefiningSchema);
            return module;
        }

        void WeaveDatabaseClass(TypeDefinition type) {
            var module = type.Module;
            var int64Type = module.ImportReference(typeof(ulong));
            var crudCreateHandle = new FieldDefinition("crudCreateHandle", FieldAttributes.Static, int64Type);
            type.Fields.Add(crudCreateHandle);

            foreach (var prop in type.Properties.Where(p => p.IsAutoImplemented())) {
            }
        }
    }
}