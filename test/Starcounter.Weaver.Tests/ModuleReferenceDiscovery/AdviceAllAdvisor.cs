
using Mono.Cecil;
using Starcounter.Weaver.Analysis;

namespace Starcounter.Weaver.Tests {
    class AdviceAllAdvisor : ModuleReferenceDiscoveryAdvisor {

        public AdviceAllAdvisor() : this(WeaverDiagnostics.Quiet) {

        }

        public AdviceAllAdvisor(WeaverDiagnostics diag) : base(diag) {

        }

        protected override bool ShouldFollowModuleFrom(TypeReference typeReference) {
            return true;
        }

        protected override bool ShouldFollowReferencesFrom(ModuleDefinition candidate) {
            return true;
        }
    }
}