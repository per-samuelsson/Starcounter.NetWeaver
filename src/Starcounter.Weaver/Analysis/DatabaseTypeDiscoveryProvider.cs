
using Mono.Cecil;

namespace Starcounter.Weaver.Analysis {

    public class DatabaseTypeDiscoveryProvider {
        readonly WeaverDiagnostics diag;

        public DatabaseTypeDiscoveryProvider(WeaverDiagnostics diagnostics) {
            diag = diagnostics;
        }

        public IDatabaseTypeDiscovery ProvideDiscovery(ModuleDefinition module, PreAnalysis preAnalysis) {
            if (preAnalysis.TargetModule == null) {
                diag.WriteWarning($"Assembly {module} does not reference given target.");
                return null;
            }

            return BeginDiscovery(module, preAnalysis);
        }

        protected virtual IDatabaseTypeDiscovery BeginDiscovery(ModuleDefinition module, PreAnalysis preAnalysis) {
            return new DefaultDatabaseTypeDiscovery();
        }
    }
}
