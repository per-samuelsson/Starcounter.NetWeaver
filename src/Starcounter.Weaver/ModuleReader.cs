using Mono.Cecil;

namespace Starcounter.Weaver {

    public abstract class ModuleReader {
        protected readonly WeaverDiagnostics diagnostics;

        protected ModuleReader(WeaverDiagnostics weaverDiagnostics) {
            Guard.NotNull(weaverDiagnostics, nameof(weaverDiagnostics));
            diagnostics = weaverDiagnostics;
        }

        public abstract ModuleDefinition Read();
    }
}