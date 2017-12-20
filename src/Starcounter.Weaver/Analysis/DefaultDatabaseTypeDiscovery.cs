
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
        
        void IDatabaseTypeDiscovery.DiscoverAssembly(AnalysisResult analysisResult) {
            analyzer.DiscoveryAssembly(analysisResult);
        }
    }
}