using Starcounter.Hosting.Schema;
using Starcounter.Weaver.Analysis;
using System.IO;

namespace Starcounter.Weaver {

    public static class WeaverBuilder {
        
        /// <summary>
        /// Creates a default weaver, weaving a given assembly file and writing
        /// the weaved result back to a given directory.
        /// </summary>
        /// <param name="assemblyFile"></param>
        /// <param name="outputDirectory"></param>
        /// <returns></returns>
        public static IWeaver BuildDefaultFromAssemblyFile(string assemblyFile, string outputDirectory) {
            Guard.FileExists(assemblyFile, nameof(assemblyFile));
            Guard.DirectoryExists(outputDirectory, nameof(outputDirectory));

            var diagnostics = WeaverDiagnostics.Quiet; // TODO:
            var readerParameters = new DefaultModuleReaderParameters(assemblyFile);
            var moduleReader = new AssemblyFileModuleReader(assemblyFile, diagnostics, readerParameters.Parameters);

            var targetPath = Path.Combine(outputDirectory, Path.GetFileName(assemblyFile));
            var moduleWriter = new AssemblyFileModuleWriter(targetPath, diagnostics);

            var schemaSerializer = new JsonNETSchemaSerializer();
            var serializationContext = new EmbeddedResourceSchemaSerializationContext(
                schemaSerializer,
                EmbeddedResourceSchemaSerializationContext.DefaultResourceStreamName,
                diagnostics
                );

            var preAnalysis = new DefaultPreAnalysis(
                new ModuleReferenceDiscovery(ModuleReferenceDiscoveryAdvisor.PossibleDefault, diagnostics),
                serializationContext,
                diagnostics
                );

            var analyzer = new ModuleAnalyzer(
                preAnalysis,
                new DatabaseTypeDiscoveryProvider(diagnostics),
                diagnostics
            );

            var weaver = new ModuleWeaver(serializationContext);

            return new AssemblyWeaver(diagnostics, moduleReader, analyzer, weaver, moduleWriter);
        }
    }
}