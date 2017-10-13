
using Mono.Cecil;
using Starcounter.Hosting.Schema;

namespace Starcounter.Weaver.Analysis {

    public abstract class SchemaSerializationContext {
        protected readonly WeaverDiagnostics diagnostics;

        protected SchemaSerializationContext(WeaverDiagnostics weaverDiagnostics) {
            Guard.NotNull(weaverDiagnostics, nameof(weaverDiagnostics));
            diagnostics = weaverDiagnostics;
        }

        public abstract DatabaseSchema Read(ModuleDefinition module);

        public abstract void Write(ModuleDefinition module, DatabaseSchema schema);
    }
}