
using Mono.Cecil;

namespace Starcounter.Weaver {

    public interface IAssemblyAnalyzer {

        bool IsTargetReference(ModuleDefinition module);

        void DiscoveryAssembly(AnalysisResult analysisResult);
    }
}