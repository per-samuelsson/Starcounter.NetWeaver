
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

        static TestUtilities() {
            var currentAssemblyPath = Assembly.GetExecutingAssembly().Location;
            currentAssemblyDefaultReaderParameters = new DefaultModuleReaderParameters(currentAssemblyPath).Parameters;
            currentAssemblyDefaultReaderParameters.ReadSymbols = false;
            currentAssemblyBytes = File.ReadAllBytes(currentAssemblyPath);
        }
        
        public static ModuleDefinition GetModuleOfCurrentAssembly(ReaderParameters readerParameters = null, bool alwaysReRead = false) {
            if (currentAssemblyModule == null || alwaysReRead) {
                currentAssemblyModule = SharedTesting.ReadTestAssembly(currentAssemblyBytes, readerParameters ?? currentAssemblyDefaultReaderParameters);
            }
            return currentAssemblyModule;
        }

        public static ModuleWithWriteTestWhenDisposed GetModuleOfCurrentAssemblyForRewriting(ReaderParameters readerParameters = null, bool discardTestWrite = false) {
            var module = SharedTesting.ReadTestAssembly(currentAssemblyBytes, readerParameters ?? currentAssemblyDefaultReaderParameters);
            return new ModuleWithWriteTestWhenDisposed(module);
        }
    }
}