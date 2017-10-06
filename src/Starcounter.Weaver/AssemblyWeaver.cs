using System;
using System.IO;
using Mono.Cecil;

#if NET_STANDARD
using Starcounter.Weaver.NetCoreAssemblyResolver;
#endif

namespace Starcounter.Weaver {

    public class AssemblyWeaver {
        public string AssemblyPath { get; private set; }
        public string WeavedAssemblyPath { get; private set; }

        public AssemblyWeaver(string assemblyFilePath, string outputDirectory) {
            if (string.IsNullOrEmpty(assemblyFilePath)) {
                throw new ArgumentNullException(nameof(assemblyFilePath));
            }

            if (!File.Exists(assemblyFilePath)) {
                throw new FileNotFoundException("Assembly not found", assemblyFilePath);
            }

            if (string.IsNullOrEmpty(outputDirectory)) {
                throw new ArgumentNullException(nameof(outputDirectory));
            }

            if (!Directory.Exists(outputDirectory)) {
                throw new DirectoryNotFoundException($"Directory not found: {outputDirectory}");
            }

            AssemblyPath = assemblyFilePath;
            WeavedAssemblyPath = Path.Combine(outputDirectory, Path.GetFileName(AssemblyPath));
        }

        public void Weave() {
            var readParameters = new ReaderParameters();
#if NET_STANDARD
            readParameters.AssemblyResolver = new DotNetCoreAssemblyResolver(AssemblyPath);
#else
            var netFrameworkResolver = new DefaultAssemblyResolver();
            netFrameworkResolver.AddSearchDirectory(Path.GetDirectoryName(AssemblyPath));
            readParameters.AssemblyResolver = netFrameworkResolver;
#endif
            var module = ModuleDefinition.ReadModule(AssemblyPath, readParameters);

            // Pass in autority that can decide if module need to be weaved and that can
            // decide of types qualify for being database classes.
            // TODO:
            var weaver = new ModuleWeaver(module);
            weaver.Weave();
            
            // TODO:
            // moduleWriter.WriteModule(...);
            module.Write(WeavedAssemblyPath);
        }
    }
}