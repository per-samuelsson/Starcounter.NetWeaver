
using Mono.Cecil;

namespace Starcounter.Weaver {

    public abstract class ModuleWriter {
        protected readonly WeaverDiagnostics diagnostics;

        protected ModuleWriter(WeaverDiagnostics weaverDiagnostics) {
            Guard.NotNull(weaverDiagnostics, nameof(weaverDiagnostics));
            diagnostics = weaverDiagnostics;
        }

        public abstract void Write(ModuleDefinition module);
    }
}