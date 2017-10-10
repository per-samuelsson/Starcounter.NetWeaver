
using Mono.Cecil;
using System.Collections.Generic;
using System.Linq;

namespace Starcounter.Weaver.Analysis {
    
    public class ModuleReferenceDiscoveryAdvisor {
        readonly WeaverDiagnostics diag;
        public readonly List<string> ExcludedTypeReferenceScopes = new List<string>();

        public ModuleReferenceDiscoveryAdvisor(WeaverDiagnostics diagnostics) {
            diag = diagnostics;
        }

        public static ModuleReferenceDiscoveryAdvisor PossibleDefault {
            get {
                var advisor = new ModuleReferenceDiscoveryAdvisor(WeaverDiagnostics.Quiet);
                advisor.ExcludedTypeReferenceScopes.Add("mscorlib");
                advisor.ExcludedTypeReferenceScopes.Add("System");
                advisor.ExcludedTypeReferenceScopes.Add("System.*");
                return advisor;
            }
        }

        internal protected virtual bool ShouldFollowModuleFrom(TypeReference typeReference) {
            var name = typeReference.Scope?.Name ?? string.Empty;

            var excluded = ExcludedTypeReferenceScopes.Any(e => {
                if (e.EndsWith("*")) {
                    var like = e.Substring(0, e.Length - 1);
                    return name.StartsWith(like);
                }
                return name.Equals(e);
            });

            return !excluded;
        }

        internal protected virtual bool ShouldFollowReferencesFrom(ModuleDefinition candidate) {
            return true;
        }
    }
}