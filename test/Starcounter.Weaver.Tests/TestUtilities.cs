using Mono.Cecil;
using System.Reflection;
using Xunit;

using ModuleDefinition = Mono.Cecil.ModuleDefinition;

namespace Starcounter.Weaver.Tests {
    public static class TestUtilities {
        static ModuleDefinition currentAssemblyModule;

        public static ModuleDefinition GetModuleOfCurrentAssembly(ReaderParameters readerParameters = null) {
            if (currentAssemblyModule == null) {
                var currentAssemblyPath = Assembly.GetExecutingAssembly().Location;
                readerParameters = readerParameters ?? new DefaultModuleReaderParameters(currentAssemblyPath).Parameters;
                var module = ModuleDefinition.ReadModule(currentAssemblyPath, readerParameters);
                Assert.NotNull(module);
                currentAssemblyModule = module;
            }
            return currentAssemblyModule;
        }
    }
}