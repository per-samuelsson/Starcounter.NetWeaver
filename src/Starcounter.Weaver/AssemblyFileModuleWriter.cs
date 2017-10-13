
using Mono.Cecil;
using System.IO;

namespace Starcounter.Weaver {

    public class AssemblyFileModuleWriter : ModuleWriter {
        protected readonly string assemblyPath;
        protected readonly WriterParameters writeParameters;

        public AssemblyFileModuleWriter(string assemblyFile, WeaverDiagnostics weaverDiagnostics, WriterParameters writerParameters = null) : base(weaverDiagnostics) {
            Guard.DirectoryExists(Path.GetDirectoryName(assemblyFile), nameof(assemblyFile));
            assemblyPath = assemblyFile;
            writeParameters = writerParameters;
        }

        // Test: path to a file in a directory that does not exist (2 levels)
        public override void Write(ModuleDefinition module) {
            Guard.NotNull(module, nameof(module));

            module.Write(assemblyPath);
        }
    }
}