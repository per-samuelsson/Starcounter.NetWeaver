
using Starcounter.Weaver;
using Starcounter.Weaver.Analysis;
using Starcounter.Weaver.Runtime;
using Starcounter.Weaver.Runtime.JsonSerializer;
using starweave.Weaver;
using System;
using System.Diagnostics;
using System.IO;

namespace starweave {

    class Program {

        public static void CheckForDebugSwitch(ref string[] args) {
            if (args.Length > 0) {
                var first = args[0].TrimStart('-');
                if (first.Equals("sc-debug", StringComparison.InvariantCultureIgnoreCase)) {
                    Debugger.Launch();
                    var stripped = new string[args.Length - 1];
                    Array.Copy(args, 1, stripped, 0, args.Length - 1);
                    args = stripped;
                }
            }
        }

        static int Main(string[] args) {
            CheckForDebugSwitch(ref args);

            // The default weaver use a strategy that identify database classes using
            // a custom attribute. Each auto-implemented property in that will be considered
            // a database property. What we need from the analyzer from the target runtime
            // is then: 1, the name of the runtime (so we can locate stuff) and 2, the type
            // of the database attribute. With that, we can carry out analysis. We expect
            // the shared database schema model (i.e. Starcounter Hosting).
            
            if (args.Length == 0) {
                Console.Error.WriteLine("Usage: starweave <assembly> [output_directory]");
                return 1;
            }

            var assemblyFile = args[0];
            if (!File.Exists(assemblyFile)) {
                Console.Error.WriteLine($"File not found: {assemblyFile}");
                Console.Error.WriteLine("Usage: starweave <assembly> [output_directory]");
                return 1;
            }

            string outputDirectory;
            if (args.Length > 1) {
                outputDirectory = args[1];
            }
            else {
                var dir = Path.GetDirectoryName(assemblyFile);
                dir = Path.Combine(dir, ".starcounter");
                outputDirectory = dir;
            }
            
            if (!Directory.Exists(outputDirectory)) {
                Directory.CreateDirectory(outputDirectory);
            }

            // Assemble the actual weaver engine
            
            var weaverFactory = new StarcounterWeaverFactory("Starcounter.dll", new DatabaseTypeStateNames());
            
            // Create diagnostics
            var diagnosticFormatter = new MsBuildAdheringFormatter("starweave");
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
            var moduleWeaver = new ModuleWeaver(serializationContext);

            // Final assemble
            var host = new DefaultWeaverHost(diagnostics);
            var weaver = new AssemblyWeaver(host, weaverFactory, moduleReader, analyzer, moduleWeaver, moduleWriter);
            
            weaver.Weave();
            Console.WriteLine($"Weaved {assemblyFile} -> {outputDirectory}");

            return 0;
        }
    }
}