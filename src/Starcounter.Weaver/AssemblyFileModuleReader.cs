
using Mono.Cecil;

namespace Starcounter.Weaver {

    public class AssemblyFileModuleReader : ModuleReader {
        protected readonly string assemblyPath;
        protected readonly ReaderParameters readParameters;

        public AssemblyFileModuleReader(string assemblyFile, WeaverDiagnostics weaverDiagnostics, ReaderParameters readerParameters = null) : base(weaverDiagnostics) {
            Guard.FileExists(assemblyFile, nameof(assemblyFile));
            assemblyPath = assemblyFile;
            readParameters = readerParameters;
        }

        public override ModuleDefinition Read() {
            return ModuleDefinition.ReadModule(assemblyPath, readParameters);
        }
    }
}