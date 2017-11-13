
using Mono.Cecil;
using Starcounter.Weaver;
using Starcounter.Weaver.Rewriting;
using System;

namespace starweave.Weaver {

    public class StarcounterWeaverFactory : IWeaverFactory {
        readonly DatabaseTypeStateNames names;
        IWeaverHost host;

        public StarcounterWeaverFactory(DatabaseTypeStateNames stateNames) {
            if (stateNames == null) {
                throw new ArgumentNullException(nameof(stateNames));
            }

            names = stateNames;
        }

        IAssemblyAnalyzer IWeaverFactory.ProvideAnalyzer(IWeaverHost weaverHost, ModuleDefinition moduleDefinition) {
            host = weaverHost;
            return new StarcounterAssemblyAnalyzer(weaverHost, moduleDefinition);
        }

        IAssemblyRewriter IWeaverFactory.ProviderRewriter(AnalysisResult analysis) {
            return new StarcounterAssemblyRewriter(host, analysis, names);
        }
    }
}