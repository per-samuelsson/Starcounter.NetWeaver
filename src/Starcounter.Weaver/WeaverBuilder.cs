
using Starcounter.Weaver.Runtime;
using Starcounter.Weaver.Analysis;
using System;
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
        public static IWeaver BuildDefaultFromAssemblyFile(string assemblyFile, string outputDirectory, IWeaverFactory weaverFactory) {
            Guard.NotNull(weaverFactory, nameof(weaverFactory));
            Guard.FileExists(assemblyFile, nameof(assemblyFile));
            Guard.DirectoryExists(outputDirectory, nameof(outputDirectory));

            // Create diagnostics
            var diagnosticFormatter = new MsBuildAdheringFormatter("Starcounter.Postcompiler");
            var diagnostics = new TextWriterWeaverDiagnostics(Console.Error, diagnosticFormatter);

            // Create module reader
            var readerParameters = new DefaultModuleReaderParameters(assemblyFile);
            var moduleReader = new AssemblyFileModuleReader(assemblyFile, diagnostics, readerParameters.Parameters);

            // Create module writer
            var targetPath = Path.Combine(outputDirectory, Path.GetFileName(assemblyFile));
            var moduleWriter = new AssemblyFileModuleWriter(targetPath, diagnostics);

            // Create serialization context
            var schemaSerializer = new JsonNETSchemaSerializer(new DefaultAdvicedContractResolver());
            var serializationContext = new EmbeddedResourceSchemaSerializationContext(
                schemaSerializer,
                EmbeddedResourceSchemaSerializationContext.DefaultResourceStreamName,
                diagnostics
                );

            // Create pre analysis, including module reference discovery
            var preAnalysis = new DefaultPreAnalysis(
                new ModuleReferenceDiscovery(ModuleReferenceDiscoveryAdvisor.PossibleDefault, diagnostics),
                serializationContext,
                diagnostics
                );

            // Now create the analyzer, and the provider of type discovery.
            var analyzer = new ModuleAnalyzer(
                preAnalysis,
                new DatabaseTypeDiscoveryProvider(diagnostics),
                diagnostics
            );

            // Create module weaver
            var weaver = new ModuleWeaver(serializationContext);

            var host = new DefaultWeaverHost(diagnostics);
            return new AssemblyWeaver(host, weaverFactory, moduleReader, analyzer, weaver, moduleWriter);
        }
    }
}