
using Mono.Cecil;

#if NET_STANDARD
using Starcounter.Weaver.NetCoreAssemblyResolver;
#else
using Starcounter.Weaver.NetFrameworkAssemblyResolver;
#endif

namespace Starcounter.Weaver {

    public class DefaultModuleReaderParameters {
        public readonly ReaderParameters Parameters;

        public DefaultModuleReaderParameters(string assemblyFile) {
            var readParameters = new ReaderParameters();
#if NET_STANDARD
            readParameters.AssemblyResolver = new DotNetCoreAssemblyResolver(assemblyFile);
#else
            readParameters.AssemblyResolver = new DotNetFrameworkAssemblyResolver(assemblyFile);
#endif
            Parameters = readParameters;
        }
    }
}