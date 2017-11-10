using Mono.Cecil;
using Starcounter.Hosting.Schema;

namespace Starcounter.Weaver.Analysis {

    public interface IDatabaseTypeDiscovery {

        void DiscoverAssembly(ModuleDefinition module, DatabaseAssembly assembly);
    }
}
