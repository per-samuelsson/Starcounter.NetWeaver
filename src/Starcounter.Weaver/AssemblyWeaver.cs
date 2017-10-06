using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Diagnostics;

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
            readParameters.InMemory = true;
#if NET_STANDARD
            readParameters.AssemblyResolver = new DotNetCoreAssemblyResolver(AssemblyPath);
#endif
            var module = ModuleDefinition.ReadModule(AssemblyPath, readParameters);
            foreach (var type in module.Types) {
                if (type.IsDatabaseType()) {
                    WeaveDatabaseClass(type);
                }
            }
            
            module.Write(WeavedAssemblyPath);
        }

        void WeaveDatabaseClass(TypeDefinition type) {
            var module = type.Module;
            var int64Type = module.ImportReference(typeof(ulong));
            var crudCreateHandle = new FieldDefinition("crudCreateHandle", FieldAttributes.Static, int64Type);
            type.Fields.Add(crudCreateHandle);

            foreach (var prop in type.Properties.Where(p => p.IsAutoImplemented())) {
                // TODO
            }
        }
    }

    static class ExtensionMethods {
        public static bool IsAutoImplemented(this PropertyDefinition p) {
            var type = p.DeclaringType;
            var name = $"<{p.Name}>k__BackingField";
            return type.HasFields && type.Fields.Any(f => f.Name == name);

            // TODO:
            // Make it more like "ShouldTransform" / Discover and check:
            //   1. It's an auto-implemented property
            //   2. Has a data type we support.
            //   3. Etc.
            //
            // We should return a materialization, including name of column,
            // datatype in starcounter, if it's a datatype not supported, maybe issue
            // some information about that, etc. If it's a code property, denote
            // that, etc.
        }

        public static bool IsDatabaseType(this TypeDefinition type) {
            var hasDatabaseAttribute = type.HasCustomAttributes && type.CustomAttributes.Any(ca => ca.AttributeType.FullName == "Starcounter.DatabaseAttribute"/*typeof(Starcounter.DatabaseAttribute).FullName*/);
            if (!hasDatabaseAttribute) {
                var tb = type.BaseType;
                if (tb != null) {
                    var baseDefinition = tb.Resolve();
                    return IsDatabaseType(baseDefinition);
                }
            }
            return hasDatabaseAttribute;
        }
    }
}