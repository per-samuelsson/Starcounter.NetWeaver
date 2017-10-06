
#if NET_STANDARD

using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.DependencyModel.Resolution;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace Starcounter.Weaver.NetCoretAssemblyResolver {

    /// <summary>
    /// .NET Core dependency walker, allowing resolving full dependency trees of
    /// assemblies.
    /// </summary>
    /// <remarks>
    /// The first implementation is based on actually loading all assemblies, advicing
    /// loader of paths. It's certainly not the optimal but work for our needs at this point.
    /// </remarks>
    public sealed class AssemblyDependencyWalker {
        readonly ICompilationAssemblyResolver assemblyResolver;
        readonly DependencyContext dependencyContext;
        readonly AssemblyLoadContext loadContext;
        readonly Action<AssemblyName, string> callback;

        /// <summary>
        /// Iterates the entire dependency tree of the given assembly and invoke
        /// <c>action</c> for each dependency detected, passing the path the dependency
        /// resolved to.
        /// </summary>
        /// <param name="assemblyFile">Full path of an assembly.</param>
        /// <param name="action">Callback invoked for each dependency detected.</param>
        public static void ForEachDependency(string assemblyFile, Action<AssemblyName, string> action) {
            var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyFile);

            var lc = AssemblyLoadContext.GetLoadContext(assembly);
            var depContext = DependencyContext.Load(assembly);
            
            var assemblyResolver = new CompositeCompilationAssemblyResolver(
                new ICompilationAssemblyResolver[] {
                    new AppBaseCompilationAssemblyResolver(Path.GetDirectoryName(assemblyFile)),
                    new ReferenceAssemblyPathResolver(),
                    new PackageCompilationAssemblyResolver()
            });

            
            
            var walker = new AssemblyDependencyWalker(lc, depContext, assemblyResolver, action);
            walker.Walk(assembly);
        }

        private AssemblyDependencyWalker(AssemblyLoadContext lc, DependencyContext dc, ICompilationAssemblyResolver ar, Action<AssemblyName, string> action) {
            loadContext = lc;
            dependencyContext = dc;
            assemblyResolver = ar;
            callback = action;
        }

        void Walk(Assembly assembly) {
            try {
                loadContext.Resolving += ResolveReference;

                var touchedAssemblies = new List<string>();
                var touchedTypes = new List<string>();
                WalkFullDependencyGraph(assembly, touchedAssemblies, touchedTypes);
            }
            finally {
                loadContext.Resolving -= ResolveReference;
            }
        }

        void WalkFullDependencyGraph(Assembly assembly, List<string> touchedAssemblies, List<string> touchedTypes) {
            if (touchedAssemblies.Contains(assembly.FullName)) {
                return;
            }

            touchedAssemblies.Add(assembly.FullName);

            foreach (var item in assembly.GetReferencedAssemblies()) {
                var x = loadContext.LoadFromAssemblyName(item);
                callback(item, x.Location);
                WalkFullDependencyGraph(x, touchedAssemblies, touchedTypes);
            }

            foreach (TypeInfo type in assembly.DefinedTypes) {
                var baseType = type.BaseType?.GetTypeInfo();
                if (baseType != null) {
                    TouchType(baseType, touchedTypes);
                }

                TouchType(type, touchedTypes);
            }
        }

        void TouchType(TypeInfo type, List<string> touchedTypes) {
            if (!touchedTypes.Contains(type.FullName)) {
                touchedTypes.Add(type.FullName);
                foreach (PropertyInfo property in type.DeclaredProperties) {
                    TouchType(property.GetType().GetTypeInfo(), touchedTypes);
                }
            }
        }

        Assembly ResolveReference(AssemblyLoadContext context, AssemblyName name) {
            var library = dependencyContext.RuntimeLibraries.FirstOrDefault(
                (candidate) => string.Equals(candidate.Name, name.Name, StringComparison.OrdinalIgnoreCase));

            if (library != null) {
                var wrapper = new CompilationLibrary(
                    library.Type,
                    library.Name,
                    library.Version,
                    library.Hash,
                    library.RuntimeAssemblyGroups.SelectMany(g => g.AssetPaths),
                    library.Dependencies,
                    library.Serviceable);

                var assemblies = new List<string>();
                assemblyResolver.TryResolveAssemblyPaths(wrapper, assemblies);
                if (assemblies.Count > 0) {
                    var resolvedPath = assemblies[0];
                    callback(name, resolvedPath);
                    return loadContext.LoadFromAssemblyPath(resolvedPath);
                }
            }

            return null;
        }
    }
}

#endif