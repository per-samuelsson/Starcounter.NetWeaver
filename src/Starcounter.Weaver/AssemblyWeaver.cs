using System;
using System.IO;
using Mono.Cecil;

namespace Starcounter.Weaver {

    public class AssemblyWeaver {
        public string AssemblyPath { get; private set; }
        public AssemblyWeaver(string assemblyFilePath) {
            if (string.IsNullOrEmpty(assemblyFilePath)) {
                throw new ArgumentNullException(nameof(assemblyFilePath));
            }

            if (!File.Exists(assemblyFilePath)) {
                throw new FileNotFoundException("Assembly not found", assemblyFilePath);
            }
            
            AssemblyPath = assemblyFilePath;
        }

        public void Weave(string outputDirectory) {
            if (string.IsNullOrEmpty(outputDirectory)) {
                throw new ArgumentNullException(nameof(outputDirectory));
            }

            if (!Directory.Exists(outputDirectory)) {
                throw new DirectoryNotFoundException($"Directory not found: {outputDirectory}");
            }

            var module = ModuleDefinition.ReadModule(AssemblyPath, new DefaultModuleReaderParameters(AssemblyPath).Parameters);

            // Pass in autority that can decide if module need to be weaved and that can
            // decide of types qualify for being database classes.
            // TODO:
            // Instead, pass in Settings to a factory, and return an interface that will
            // return the weaved module.
            // var weaver = ModuleWeaverFactory.CreateWeaver(module, settings);
            // var result = weaver.Weave();
            var weaver = new ModuleWeaver(module);
            var weavedModule = weaver.Weave();

            // TODO:
            // moduleWriter.WriteModule(...);
            var targetPath = Path.Combine(outputDirectory, Path.GetFileName(AssemblyPath));
            weavedModule.Write(targetPath);
        }
    }
}