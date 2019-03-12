
namespace Starcounter.Weaver {

    public class AssemblyWeaver : IWeaver {
        readonly IWeaverHost host;
        readonly IWeaverFactory factory;
        readonly ModuleReader reader;
        readonly ModuleAnalyzer analyzer;
        readonly ModuleWeaver weaver;
        readonly ModuleWriter writer;
        
        public AssemblyWeaver(
            IWeaverHost weaverHost,
            IWeaverFactory weaverFactory,
            ModuleReader moduleReader,
            ModuleAnalyzer moduleAnalyzer,
            ModuleWeaver moduleWeaver,
            ModuleWriter moduleWriter) {

            Guard.NotNull(weaverHost, nameof(weaverHost));
            Guard.NotNull(weaverFactory, nameof(weaverFactory));
            Guard.NotNull(moduleReader, nameof(moduleReader));
            Guard.NotNull(moduleAnalyzer, nameof(moduleAnalyzer));
            Guard.NotNull(moduleWeaver, nameof(moduleWeaver));
            Guard.NotNull(moduleWriter, nameof(moduleWriter));

            host = weaverHost;
            factory = weaverFactory;
            reader = moduleReader;
            analyzer = moduleAnalyzer;
            weaver = moduleWeaver;
            writer = moduleWriter;
        }

        public void Weave() {
            var module = reader.Read();

            var assemblyAnalyzer = factory.ProvideAnalyzer(host, module);

            var analysisResult = analyzer.Analyze(module, assemblyAnalyzer);
            if (analysisResult != null) {
                var rewriter = factory.ProvideRewriter(analysisResult);
                if (rewriter != null) {
                    var weavedModule = weaver.Weave(analysisResult, rewriter);
                    writer.Write(module);
                }
            }
        }
    }
}