
using Mono.Cecil;

#if NET_STANDARD
using Starcounter.Weaver.NetCoreAssemblyResolver;
#endif

namespace Starcounter.Weaver {

    public class DefaultModuleReaderParameters {
        public readonly ReaderParameters Parameters;

        public DefaultModuleReaderParameters(string assemblyFile) {
            var readParameters = new ReaderParameters();
#if NET_STANDARD
            readParameters.AssemblyResolver = new DotNetCoreAssemblyResolver(assemblyFile);
#else
            var netFrameworkResolver = new DefaultAssemblyResolver();
            netFrameworkResolver.AddSearchDirectory(System.IO.Path.GetDirectoryName(assemblyFile));
            readParameters.AssemblyResolver = netFrameworkResolver;
#endif
            Parameters = readParameters;
        }
    }
}