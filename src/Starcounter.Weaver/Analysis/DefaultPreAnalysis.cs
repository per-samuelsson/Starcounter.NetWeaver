
using Mono.Cecil;
using Starcounter.Weaver.Runtime;

namespace Starcounter.Weaver.Analysis {

    public class DefaultPreAnalysis : PreAnalysis {
        
        public DefaultPreAnalysis(ModuleReferenceDiscovery moduleReferenceDiscovery, SchemaSerializationContext serializationContext, WeaverDiagnostics diagnostics) : base(moduleReferenceDiscovery, serializationContext, diagnostics) {
        }
        
        protected override DatabaseSchema DiscoverSchema(ModuleDefinition candidate, SchemaSerializationContext serializationContext) {
            return serializationContext.Read(candidate);
        }
    }
}