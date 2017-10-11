
using Mono.Cecil;
using System;
using System.Collections.Generic;

namespace Starcounter.Weaver.Analysis {

    public sealed class SingleModuleReferenceFinder {
        public ModuleDefinition Result { get; private set; }

        public IEnumerable<ModuleDefinition> ModulesConsidered { get; private set; }

        private SingleModuleReferenceFinder() {
            Result = null;
        }

        public static SingleModuleReferenceFinder Run(ModuleDefinition candidate, ModuleReferenceDiscovery discovery, Func<ModuleDefinition, bool> predicate) {
            Guard.NotNull(candidate, nameof(candidate));
            Guard.NotNull(predicate, nameof(predicate));
            Guard.NotNull(discovery, nameof(discovery));

            var finder = new SingleModuleReferenceFinder();
            var discovered = new List<ModuleDefinition>();
            discovery.DiscoverReferences(candidate, (m) => {
                if (predicate(m)) {
                    if (finder.Result != null) {
                        throw new Exception($"Multiple results. Cant assign {m} as {nameof(Result)}; it's already assingned to {finder.Result}");
                    }

                    finder.Result = m;
                }
                discovered.Add(m);
                return true;
            });

            finder.ModulesConsidered = discovered;
            return finder;
        }
    }
}
