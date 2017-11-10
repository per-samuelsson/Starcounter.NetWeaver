
using Mono.Cecil;
using Starcounter.Hosting.Schema;

namespace Starcounter.Weaver.Analysis {

    public class DefaultDatabaseTypeDiscovery : IDatabaseTypeDiscovery {
        readonly WeaverDiagnostics diagnostics;
        readonly IAssemblyAnalyzer analyzer;
        
        public DefaultDatabaseTypeDiscovery(WeaverDiagnostics weaverDiagnostics, IAssemblyAnalyzer assemblyAnalyzer) {
            Guard.NotNull(weaverDiagnostics, nameof(weaverDiagnostics));
            Guard.NotNull(assemblyAnalyzer, nameof(assemblyAnalyzer));

            diagnostics = weaverDiagnostics;
            analyzer = assemblyAnalyzer;
        }
        
        void IDatabaseTypeDiscovery.DiscoverAssembly(ModuleDefinition module, DatabaseAssembly assembly) {
            analyzer.DiscoveryAssembly(assembly);
        }
    }
}