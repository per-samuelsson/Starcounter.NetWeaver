
using Mono.Cecil;
using Starcounter.Hosting.Schema;

namespace Starcounter.Weaver.Analysis {

    public class DatabaseTypeDiscoveryProvider {
        readonly WeaverDiagnostics diag;

        public DatabaseTypeDiscoveryProvider(WeaverDiagnostics diagnostics) {
            diag = diagnostics;
        }

        public IDatabaseTypeDiscovery ProvideDiscovery(ModuleDefinition module, ModuleDefinition preAnalysisTarget, DatabaseSchema preAnalysisSchema) {
            if (preAnalysisTarget == null) {
                diag.WriteWarning($"Assembly {module} does not reference given target.");
                return null;
            }

            return BeginDiscovery(module);
        }

        protected virtual IDatabaseTypeDiscovery BeginDiscovery(ModuleDefinition module) {
            return new DefaultDatabaseTypeDiscovery();
        }
    }
}
