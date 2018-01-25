
using Mono.Cecil;
using Starcounter.Weaver;
using System;
using System.IO;
using System.Linq;
using Xunit;

namespace SharedTestUtilities {

    public static class SharedTesting {
        static WeaverDiagnostics quietDiagnostics;

        public static WeaverDiagnostics QuietDiagnostics {
            get {
                if (quietDiagnostics == null) {
                    quietDiagnostics = WeaverDiagnostics.Quiet;
                }
                return quietDiagnostics;
            }
        }

        public static ModuleDefinition ReadTestAssembly(byte[] testAssembly, ReaderParameters readerParameters) {
            var module = ModuleDefinition.ReadModule(new MemoryStream(testAssembly, false), readerParameters);
            Assert.NotNull(module);
            return module;
        }

        public static TypeDefinition DefinitionOf(this ModuleDefinition module, Type type) {
            // Using First would be faster, but lets stick to Single as an
            // extra testing measurement.
            var def = module.Types.Single(t => t.MetadataToken.ToInt32() == type.MetadataToken);
            Assert.NotNull(def);
            return def;

        }
    }
}