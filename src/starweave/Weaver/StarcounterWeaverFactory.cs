
using Mono.Cecil;
using Starcounter.Weaver;

namespace starweave.Weaver {

    public class StarcounterWeaverFactory : IWeaverFactory {
        IWeaverHost host;

        IAssemblyAnalyzer IWeaverFactory.ProvideAnalyzer(IWeaverHost weaverHost, ModuleDefinition moduleDefinition) {
            host = weaverHost;
            return new StarcounterAssemblyAnalyzer(weaverHost, moduleDefinition);
        }

        IAssemblyRewriter IWeaverFactory.ProviderRewriter(AnalysisResult analysis) {
            return new StarcounterAssemblyRewriter(host, analysis);
        }
    }
}