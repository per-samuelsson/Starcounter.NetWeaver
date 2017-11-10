
using Mono.Cecil;
using Starcounter.Hosting.Schema;

namespace Starcounter.Weaver {

    public interface IAssemblyAnalyzer {

        bool IsTargetReference(ModuleDefinition module);

        void DiscoveryAssembly(DatabaseAssembly assembly);
    }
}