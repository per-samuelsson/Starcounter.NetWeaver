
using Mono.Cecil;
using Starcounter2;
using System.Linq;

namespace Starcounter.Weaver {

    internal class ModuleWeaver {
        readonly ModuleDefinition module;

        public ModuleWeaver(ModuleDefinition module) {
            this.module = module;
        }

        public ModuleDefinition Weave() {
            foreach (var type in module.Types) {
                if (type.HasCustomAttribute(typeof(DatabaseAttribute))) {
                    WeaveDatabaseClass(type);
                }
            }
            return module;
        }

        void WeaveDatabaseClass(TypeDefinition type) {
            var module = type.Module;
            var int64Type = module.ImportReference(typeof(ulong));
            var crudCreateHandle = new FieldDefinition("crudCreateHandle", FieldAttributes.Static, int64Type);
            type.Fields.Add(crudCreateHandle);

            foreach (var prop in type.Properties.Where(p => p.IsAutoImplemented())) {
                // TODO:
                // Instead of simple IsAutoImplemented:
                // Make it more like "ShouldTransform" / Discover and check:
                //   1. It's an auto-implemented property
                //   2. Has a data type we support.
                //   3. Etc.
                //
                // We should return a materialization, including name of column,
                // datatype in starcounter, if it's a datatype not supported, maybe issue
                // some information about that, etc. If it's a code property, denote
                // that, etc.
            }
        }
    }
}
