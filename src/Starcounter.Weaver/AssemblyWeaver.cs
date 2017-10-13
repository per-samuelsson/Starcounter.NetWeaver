
namespace Starcounter.Weaver {

    public class AssemblyWeaver : IWeaver {
        readonly WeaverDiagnostics diagnostics;
        readonly ModuleReader reader;
        readonly ModuleAnalyzer analyzer;
        readonly ModuleWeaver weaver;
        readonly ModuleWriter writer;
        
        public AssemblyWeaver(
            WeaverDiagnostics weaverDiagnostics,
            ModuleReader moduleReader,
            ModuleAnalyzer moduleAnalyzer,
            ModuleWeaver moduleWeaver,
            ModuleWriter moduleWriter) {

            Guard.NotNull(weaverDiagnostics, nameof(weaverDiagnostics));
            Guard.NotNull(moduleReader, nameof(moduleReader));
            Guard.NotNull(moduleAnalyzer, nameof(moduleAnalyzer));
            Guard.NotNull(moduleWeaver, nameof(moduleWeaver));
            Guard.NotNull(moduleWriter, nameof(moduleWriter));

            diagnostics = weaverDiagnostics;
            reader = moduleReader;
            analyzer = moduleAnalyzer;
            weaver = moduleWeaver;
            writer = moduleWriter;
        }

        public void Weave() {
            var module = reader.Read();

            var assembly = analyzer.DiscoverAssembly(module);
            if (assembly != null) {
                var weavedModule = weaver.Weave(module, assembly);
                writer.Write(module);
            }
        }
    }
}