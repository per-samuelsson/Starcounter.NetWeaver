
using Mono.Cecil;
using Starcounter.Hosting.Schema;

namespace Starcounter.Weaver {

    public interface IAssemblyAnalyzer {

        bool IsTargetReference(ModuleDefinition module);

        DatabaseSchema DiscoveryAssembly(DatabaseAssembly assembly);
    }
}