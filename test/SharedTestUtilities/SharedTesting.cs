
using Mono.Cecil;
using Starcounter.Weaver;
using System.IO;
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
    }
}