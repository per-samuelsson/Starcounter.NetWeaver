using Mono.Cecil;
using Starcounter.Weaver.Analysis;
using System.Reflection;
using Xunit;

using ModuleDefinition = Mono.Cecil.ModuleDefinition;

namespace Starcounter.Weaver.Tests {
    public static class TestUtilities {
        static ModuleDefinition currentAssemblyModule;
        static WeaverDiagnostics quietDiagnostics;
        static ModuleReferenceDiscovery adviceAllReferenceDiscovery;

        public static WeaverDiagnostics QuietDiagnostics {
            // Use this. We don't now if we want to keep WeaverDiagnostics.Quiet.
            get {
                if (quietDiagnostics == null) {
                    quietDiagnostics = WeaverDiagnostics.Quiet;
                }
                return quietDiagnostics;
            }
        }

        public static ModuleReferenceDiscovery AdviceAllReferenceDiscovery {
            get {
                if (adviceAllReferenceDiscovery == null) {
                    var diag = TestUtilities.QuietDiagnostics;
                    adviceAllReferenceDiscovery = new ModuleReferenceDiscovery(new AdviceAllAdvisor(diag), diag);
                }
                return adviceAllReferenceDiscovery;
            }
        }

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