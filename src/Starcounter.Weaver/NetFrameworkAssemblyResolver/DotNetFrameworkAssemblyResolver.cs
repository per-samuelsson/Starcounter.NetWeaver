
#if !NET_STANDARD
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Text;

namespace Starcounter.Weaver.NetFrameworkAssemblyResolver {

    class DotNetFrameworkAssemblyResolver : DefaultAssemblyResolver {

        public DotNetFrameworkAssemblyResolver(string assemblyFile) {
            AddSearchDirectory(System.IO.Path.GetDirectoryName(assemblyFile));
        }
    }
}
#endif