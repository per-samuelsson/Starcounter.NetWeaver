using Mono.Cecil;
using Starcounter.Hosting.Schema;

namespace Starcounter.Weaver.Analysis {

    public abstract class PreAnalysis {

        public ModuleDefinition TargetModule { get; private set; }

        public DatabaseSchema ExternalSchema { get; private set; }

        protected PreAnalysis() {
            TargetModule = null;
            ExternalSchema = null;
        }

        public static PreAnalysis Execute<T>(
            ModuleDefinition module,
            ModuleReferenceDiscovery referenceDiscovery,
            ISchemaSerializer schemaSerializer,
            WeaverDiagnostics diagnostics) where T : PreAnalysis, new() {
            
            var preAnalysis = new T();

            var referenceFinder = SingleModuleReferenceFinder.Run(module, referenceDiscovery, (m) => {
                return preAnalysis.IsTargetModule(m);
            });

            if (referenceFinder.Result == null) {
                return preAnalysis;
            }

            preAnalysis.TargetModule = referenceFinder.Result;

            var externalSchema = new DatabaseSchema();

            foreach (var referenceDiscovered in referenceFinder.ModulesConsidered) {
                var moduleSchema = preAnalysis.DiscoverSchema(referenceDiscovered, schemaSerializer);
                if (moduleSchema != null) {
                    externalSchema = externalSchema.MergeWith(moduleSchema);
                }
            }

            preAnalysis.ExternalSchema = externalSchema;
            return preAnalysis;
        }

        protected abstract bool IsTargetModule(ModuleDefinition candidate);

        protected abstract DatabaseSchema DiscoverSchema(ModuleDefinition candidate, ISchemaSerializer serializer);
    }
}
