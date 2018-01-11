
using Mono.Cecil;
using Starcounter.Weaver;
using System;

namespace starweave.Weaver {

    public class StarcounterWeaverFactory : IWeaverFactory {
        readonly string targetAssemblyIdentity;
        readonly DatabaseTypeStateNames names;
        IWeaverHost host;
        StarcounterAssemblyAnalyzer analyzer;

        public StarcounterWeaverFactory(string runtimeTargetAssemblyIdentity, DatabaseTypeStateNames stateNames) {
            targetAssemblyIdentity = runtimeTargetAssemblyIdentity ?? throw new ArgumentNullException(nameof(runtimeTargetAssemblyIdentity));
            names = stateNames ?? throw new ArgumentNullException(nameof(stateNames));
        }

        IAssemblyAnalyzer IWeaverFactory.ProvideAnalyzer(IWeaverHost weaverHost, ModuleDefinition moduleDefinition) {
            host = weaverHost;
            var runtimeProvider = new AssemblyLoadTargetRuntimeProvider(weaverHost, targetAssemblyIdentity);
            analyzer = new StarcounterAssemblyAnalyzer(weaverHost, moduleDefinition, runtimeProvider);
            return analyzer;
        }

        IAssemblyRewriter IWeaverFactory.ProviderRewriter(AnalysisResult analysis) {
            return new StarcounterAssemblyRewriter(host, analysis, analyzer.RuntimeFacade, names);
        }
    }
}