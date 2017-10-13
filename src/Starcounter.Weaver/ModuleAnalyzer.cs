
using Mono.Cecil;
using Starcounter.Hosting.Schema;
using Starcounter.Weaver.Analysis;

namespace Starcounter.Weaver {

    internal class ModuleAnalyzer {
        readonly PreAnalysis preAnalysis;
        readonly WeaverDiagnostics diag;
        readonly DatabaseTypeDiscoveryProvider databaseDiscoveryProvider;
        
        public ModuleAnalyzer(
            PreAnalysis preAnalyser,
            DatabaseTypeDiscoveryProvider databaseTypeDiscovery,
            WeaverDiagnostics diagnostics) {

            Guard.NotNull(preAnalyser, nameof(preAnalyser));
            Guard.NotNull(databaseTypeDiscovery, nameof(databaseTypeDiscovery));
            Guard.NotNull(diagnostics, nameof(diagnostics));

            preAnalysis = preAnalyser;
            diag = diagnostics;
            databaseDiscoveryProvider = databaseTypeDiscovery;
        }
        
        public DatabaseAssembly DiscoverAssembly(ModuleDefinition module) {
            ModuleDefinition targetReference;
            DatabaseSchema schema;
            preAnalysis.Execute(module, out targetReference, out schema);

            var databaseDiscovery = databaseDiscoveryProvider.ProvideDiscovery(module, targetReference, schema);
            if (databaseDiscovery == null) {
                return null;
            }
            
            var assembly = schema.DefineAssembly(module.Name);

            databaseDiscovery.DiscoverAssembly(module, assembly);

            return assembly;
        }
    }
}