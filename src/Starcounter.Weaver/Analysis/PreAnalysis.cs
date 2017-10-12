using Mono.Cecil;
using Starcounter.Hosting.Schema;

namespace Starcounter.Weaver.Analysis {

    public abstract class PreAnalysis {
        readonly ModuleReferenceDiscovery refDiscovery;
        readonly ISchemaSerializer serializer;
        readonly WeaverDiagnostics diag;
        
        protected PreAnalysis(
            ModuleReferenceDiscovery referenceDiscovery,
            ISchemaSerializer schemaSerializer,
            WeaverDiagnostics diagnostics) {

            refDiscovery = referenceDiscovery;
            serializer = schemaSerializer;
            diag = diagnostics;
        }

        public void Execute(ModuleDefinition module, out ModuleDefinition target, out DatabaseSchema externalSchema) {
            target = null;
            externalSchema = null;

            var referenceFinder = SingleModuleReferenceFinder.Run(module, refDiscovery, (m) => {
                return IsTargetModule(m);
            });

            if (referenceFinder.Result == null) {
                return;
            }

            target = referenceFinder.Result;
            externalSchema = new DatabaseSchema();

            foreach (var referenceDiscovered in referenceFinder.ModulesConsidered) {
                var moduleSchema = DiscoverSchema(referenceDiscovered, serializer);
                if (moduleSchema != null) {
                    externalSchema = externalSchema.MergeWith(moduleSchema);
                }
            }
        }

        protected abstract bool IsTargetModule(ModuleDefinition candidate);

        protected abstract DatabaseSchema DiscoverSchema(ModuleDefinition candidate, ISchemaSerializer serializer);
    }
}
