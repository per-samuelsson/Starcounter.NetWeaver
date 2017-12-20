
using Mono.Cecil;
using Starcounter.Weaver;
using Starcounter2.Internal;
using System;

namespace starweave.Weaver {

    public class StarcounterWeaverFactory : IWeaverFactory {
        readonly string targetAssemblyIdentity;
        readonly DatabaseTypeStateNames names;
        readonly DbCrudMethodProvider methodProvider;
        IWeaverHost host;
        StarcounterAssemblyAnalyzer analyzer;

        public StarcounterWeaverFactory(string runtimeTargetAssemblyIdentity, DatabaseTypeStateNames stateNames, DbCrudMethodProvider crudMethods) {
            targetAssemblyIdentity = runtimeTargetAssemblyIdentity ?? throw new ArgumentNullException(nameof(runtimeTargetAssemblyIdentity));
            names = stateNames ?? throw new ArgumentNullException(nameof(stateNames));
            methodProvider = crudMethods ?? throw new ArgumentNullException(nameof(crudMethods));
        }

        IAssemblyAnalyzer IWeaverFactory.ProvideAnalyzer(IWeaverHost weaverHost, ModuleDefinition moduleDefinition) {
            host = weaverHost;
            var runtimeProvider = new AssemblyLoadTargetRuntimeProvider(weaverHost, targetAssemblyIdentity);
            analyzer = new StarcounterAssemblyAnalyzer(weaverHost, moduleDefinition, runtimeProvider, methodProvider.SupportedDataTypes);
            return analyzer;
        }

        IAssemblyRewriter IWeaverFactory.ProviderRewriter(AnalysisResult analysis) {
            return new StarcounterAssemblyRewriter(host, analysis, analyzer.RuntimeFacade, names);
        }
    }
}