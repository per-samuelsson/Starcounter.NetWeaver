
using Mono.Cecil;
using Starcounter.Weaver;
using Starcounter.Weaver.Rewriting;
using Starcounter2.Internal;
using System;

namespace starweave.Weaver {

    public class StarcounterWeaverFactory : IWeaverFactory {
        readonly DatabaseTypeStateNames names;
        readonly CRUDMethodProvider methodProvider;
        IWeaverHost host;

        public StarcounterWeaverFactory(DatabaseTypeStateNames stateNames, CRUDMethodProvider crudMethods) {
            names = stateNames ?? throw new ArgumentNullException(nameof(stateNames));
            methodProvider = crudMethods ?? throw new ArgumentNullException(nameof(crudMethods));
        }

        IAssemblyAnalyzer IWeaverFactory.ProvideAnalyzer(IWeaverHost weaverHost, ModuleDefinition moduleDefinition) {
            host = weaverHost;
            return new StarcounterAssemblyAnalyzer(weaverHost, moduleDefinition, methodProvider.SupportedDataTypes);
        }

        IAssemblyRewriter IWeaverFactory.ProviderRewriter(AnalysisResult analysis) {
            return new StarcounterAssemblyRewriter(host, analysis, names, methodProvider);
        }
    }
}