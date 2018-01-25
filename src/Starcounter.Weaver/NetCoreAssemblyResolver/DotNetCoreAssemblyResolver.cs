
#if NET_STANDARD
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.DependencyModel.Resolution;
using Mono.Cecil;
using Starcounter.Weaver.NetCoretAssemblyResolver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;

namespace Starcounter.Weaver.NetCoreAssemblyResolver {
    
    class DotNetCoreAssemblyResolver : IAssemblyResolver {
        Dictionary<string, Lazy<AssemblyDefinition>> referencedAssemblies;
        
        public DotNetCoreAssemblyResolver(string assemblyFile) {
            referencedAssemblies = new Dictionary<string, Lazy<AssemblyDefinition>>();

            AssemblyDependencyWalker.ForEachDependency(assemblyFile, (name, path) => {
                if (!referencedAssemblies.ContainsKey(name.FullName)) {
                    referencedAssemblies.Add(name.FullName, new Lazy<AssemblyDefinition>(() => AssemblyDefinition.ReadAssembly(path, new ReaderParameters() { AssemblyResolver = this })));
                }
            });
        }

        public virtual AssemblyDefinition Resolve(string fullName) {
            return Resolve(fullName, new ReaderParameters());
        }

        public virtual AssemblyDefinition Resolve(string fullName, ReaderParameters parameters) {
            if (fullName == null)
                throw new ArgumentNullException("fullName");

            return Resolve(AssemblyNameReference.Parse(fullName), parameters);
        }

        public AssemblyDefinition Resolve(AssemblyNameReference name) {
            return Resolve(name, new ReaderParameters());
        }

        public AssemblyDefinition Resolve(AssemblyNameReference name, ReaderParameters parameters) {
            if (name == null) {
                throw new ArgumentNullException("name");
            }

            if (referencedAssemblies.TryGetValue(name.FullName, out Lazy<AssemblyDefinition> asm)) {
                return asm.Value;
            }

            // Investigate alternative strategy here. Keep underlying walker of
            // AssemblyDependencyWalker around, and instead resolve only those references
            // actually needed by Cecil runtime, instead of solving entire tree upfront
            // as we do currently.
            // TODO:

            throw new AssemblyResolutionException(name);
        }

        protected virtual void Dispose(bool disposing) {
            if (!disposing)
                return;

            foreach (var lazy in referencedAssemblies.Values) {
                if (!lazy.IsValueCreated)
                    continue;

                lazy.Value.Dispose();
            }
        }

        public void Dispose() {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
#endif
