
using Mono.Cecil;
using Starcounter.Hosting.Schema;

namespace Starcounter.Weaver.Analysis {

    public class DefaultPreAnalysis : PreAnalysis {

        // Capture Starcounter module and all modules referenced that contains
        // a schema. These modules are those that interest us.
        //
        // Ideally, we also want to provide a warning for any reference that
        //   1. dont contain a schema, AND
        //   2. still reference Starcounter
        // That could be an indication it's not weaved, but should have been.

        public DefaultPreAnalysis(ModuleReferenceDiscovery moduleReferenceDiscovery, ISchemaSerializer schemaSerializer, WeaverDiagnostics diagnostics) : base(moduleReferenceDiscovery, schemaSerializer, diagnostics) {
        }

        protected override bool IsTargetModule(ModuleDefinition candidate) {
            return candidate.Name.Equals("Starcounter2.dll");
        }

        protected override DatabaseSchema DiscoverSchema(ModuleDefinition candidate, ISchemaSerializer serializer) {
            var embeddedSchemaData = candidate.ReadEmbeddedResource(SchemaSerialization.EmbeddedResourceStreamName);
            return embeddedSchemaData != null ? serializer.Deserialize(embeddedSchemaData) : null;
        }
    }
}