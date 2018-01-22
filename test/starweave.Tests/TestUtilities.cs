
using Mono.Cecil;
using Starcounter.Weaver;
using System.IO;
using System.Reflection;
using Xunit;
using SharedTestUtilities;

using ModuleDefinition = Mono.Cecil.ModuleDefinition;

namespace starweave.Weaver.Tests {

    public static class TestUtilities {
        static byte[] currentAssemblyBytes;
        static ModuleDefinition currentAssemblyModule;
        static ReaderParameters currentAssemblyDefaultReaderParameters;
        static WeaverDiagnostics quietDiagnostics;

        static TestUtilities() {
            var currentAssemblyPath = Assembly.GetExecutingAssembly().Location;
            currentAssemblyDefaultReaderParameters = new DefaultModuleReaderParameters(currentAssemblyPath).Parameters;
            currentAssemblyDefaultReaderParameters.ReadSymbols = false;
            currentAssemblyBytes = File.ReadAllBytes(currentAssemblyPath);
        }
        
        public static ModuleDefinition GetModuleOfCurrentAssembly(ReaderParameters readerParameters = null, bool alwaysReRead = false) {
            if (currentAssemblyModule == null || alwaysReRead) {
                readerParameters = readerParameters ?? currentAssemblyDefaultReaderParameters;
                var module = ModuleDefinition.ReadModule(new MemoryStream(currentAssemblyBytes, false), readerParameters);
                Assert.NotNull(module);
                currentAssemblyModule = module;
            }
            return currentAssemblyModule;
        }

        public static ModuleWithWriteTestWhenDisposed GetModuleOfCurrentAssemblyForRewriting(ReaderParameters readerParameters = null, bool discardTestWrite = false) {
            readerParameters = readerParameters ?? currentAssemblyDefaultReaderParameters;
            var module = ModuleDefinition.ReadModule(new MemoryStream(currentAssemblyBytes, false), readerParameters);
            Assert.NotNull(module);
            return new ModuleWithWriteTestWhenDisposed(module);
        }
    }
}