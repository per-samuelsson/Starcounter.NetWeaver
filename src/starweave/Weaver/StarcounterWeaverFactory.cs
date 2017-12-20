
using Mono.Cecil;
using Starcounter.Weaver;
using Starcounter2.Internal;
using System;

namespace starweave.Weaver {

    public class StarcounterWeaverFactory : IWeaverFactory {
        readonly string target;
        readonly DatabaseTypeStateNames names;
        readonly DbCrudMethodProvider methodProvider;
        IWeaverHost host;

        public StarcounterWeaverFactory(string targetAssembly, DatabaseTypeStateNames stateNames, DbCrudMethodProvider crudMethods) {
            target = targetAssembly ?? throw new ArgumentNullException(nameof(targetAssembly));
            names = stateNames ?? throw new ArgumentNullException(nameof(stateNames));
            methodProvider = crudMethods ?? throw new ArgumentNullException(nameof(crudMethods));
        }

        IAssemblyAnalyzer IWeaverFactory.ProvideAnalyzer(IWeaverHost weaverHost, ModuleDefinition moduleDefinition) {
            host = weaverHost;
            var runtimeProvider = new AssemblyLoadTargetRuntimeProvider(weaverHost, target);
            return new StarcounterAssemblyAnalyzer(weaverHost, moduleDefinition, runtimeProvider, methodProvider.SupportedDataTypes);
        }

        IAssemblyRewriter IWeaverFactory.ProviderRewriter(AnalysisResult analysis) {
            return new StarcounterAssemblyRewriter(host, analysis, names, methodProvider);
        }
    }
}