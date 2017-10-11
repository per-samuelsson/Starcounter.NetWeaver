using Mono.Cecil;
using Starcounter.Weaver.Analysis;

namespace Starcounter.Weaver.Tests {
    class AdviceNoneAdvisor : ModuleReferenceDiscoveryAdvisor {

        public AdviceNoneAdvisor() : this(WeaverDiagnostics.Quiet) {

        }

        public AdviceNoneAdvisor(WeaverDiagnostics diag) : base(diag) {

        }

        protected override bool ShouldFollowModuleFrom(TypeReference typeReference) {
            return false;
        }

        protected override bool ShouldFollowReferencesFrom(ModuleDefinition candidate) {
            return false;
        }
    }
}
