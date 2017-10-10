﻿
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Starcounter.Weaver.Analysis {
    
    public class ModuleReferenceDiscovery {
        readonly WeaverDiagnostics diagnostics;
        readonly ModuleDefinition module;
        readonly ModuleReferenceDiscoveryAdvisor advisor;
        
        public ModuleReferenceDiscovery(ModuleDefinition m, ModuleReferenceDiscoveryAdvisor advice, WeaverDiagnostics diag) {
            Guard.NotNull(m, nameof(m));
            Guard.NotNull(advice, nameof(advice));
            Guard.NotNull(diag, nameof(diag));
            module = m;
            advisor = advice;
            diagnostics = diag;
        }

        public void DiscoverReferences(Func<ModuleDefinition, bool> func) {
            Guard.NotNull(func, nameof(func));

            var referencedModules = new List<ModuleDefinition>();
            referencedModules.Add(module);

            DiscoverFromReferences(module.GetTypeReferences(), referencedModules, func);
        }

        bool DiscoverFromReferences(IEnumerable<TypeReference> references, List<ModuleDefinition> discoveredModules, Func<ModuleDefinition, bool> func) {
            foreach (var tref in references) {
                if (!advisor.ShouldFollowModuleFrom(tref)) {
                    Trace($"Adviced not to follow reference {tref}");
                    continue;
                }

                var m = tref.Resolve().Module;

                if (discoveredModules.Any(m2 => m2.FileName == m.FileName)) {
                    continue;
                }

                Trace($"Discovered module {m}");
                discoveredModules.Add(m);
                var keepDiscovering = func(m);
                if (!keepDiscovering) {
                    Trace($"Adviced to abort further discovery from callback");
                    return false;
                }

                if (!advisor.ShouldFollowReferencesFrom(m)) {
                    Trace($"Adviced not to follow references from module {m}");
                    continue;
                }

                if (!DiscoverFromReferences(m.GetTypeReferences(), discoveredModules, func)) {
                    return false;
                }
            }
            return true;
        }

        void Trace(string msg) {
            diagnostics.Trace($"{GetType().Name}: {msg}");
        }
    }
}