
using Mono.Cecil;
using Starcounter.Weaver.Runtime;

namespace Starcounter.Weaver.Analysis {

    public class DatabaseTypeDiscoveryProvider {
        readonly WeaverDiagnostics diagnostics;

        public DatabaseTypeDiscoveryProvider(WeaverDiagnostics weaverDiagnostics) {
            Guard.NotNull(weaverDiagnostics, nameof(weaverDiagnostics));
            diagnostics = weaverDiagnostics;
        }

        public IDatabaseTypeDiscovery ProvideDiscovery(ModuleDefinition module, IAssemblyAnalyzer assemblyAnalyzer, ModuleDefinition preAnalysisTarget, DatabaseSchema preAnalysisSchema) {
            if (preAnalysisTarget == null) {
                diagnostics.WriteWarning($"Assembly {module} does not reference given target.");
                return null;
            }

            return BeginDiscovery(module, assemblyAnalyzer);
        }

        protected virtual IDatabaseTypeDiscovery BeginDiscovery(ModuleDefinition module, IAssemblyAnalyzer assemblyAnalyzer) {
            return new DefaultDatabaseTypeDiscovery(diagnostics, assemblyAnalyzer);
        }
    }
}