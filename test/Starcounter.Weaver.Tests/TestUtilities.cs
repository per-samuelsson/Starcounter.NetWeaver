using Mono.Cecil;
using Starcounter.Hosting.Schema;
using Starcounter.Hosting.Schema.Serialization;
using Starcounter.Weaver.Analysis;
using System;
using System.IO;
using System.Reflection;
using Xunit;

using ModuleDefinition = Mono.Cecil.ModuleDefinition;

namespace Starcounter.Weaver.Tests {

    public class ModuleWithWriteTestWhenDisposed : IDisposable {
        ModuleDefinition module;
        bool discardWrite;

        public ModuleDefinition Module {
            get {
                return module;
            }
        }

        public ModuleWithWriteTestWhenDisposed(ModuleDefinition moduleDefinition, bool discardTestWrite = false) {
            module = moduleDefinition;
            discardWrite = discardTestWrite;
        }

        public void Dispose() {
            if (!discardWrite) {
                module?.Write(new MemoryStream());
            }
            module = null;
        }
    }

    public static class TestUtilities {
        static byte[] currentAssemblyBytes;
        static ModuleDefinition currentAssemblyModule;
        static ReaderParameters currentAssemblyDefaultReaderParameters;
        static WeaverDiagnostics quietDiagnostics;
        static ModuleReferenceDiscovery adviceAllReferenceDiscovery;
        static ModuleReferenceDiscovery adviceNoneReferenceDiscovery;

        static TestUtilities() {
            var currentAssemblyPath = Assembly.GetExecutingAssembly().Location;
            currentAssemblyDefaultReaderParameters = new DefaultModuleReaderParameters(currentAssemblyPath).Parameters;
            currentAssemblyBytes = File.ReadAllBytes(currentAssemblyPath);
        }

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

        public static ModuleReferenceDiscovery AdviceNoneReferenceDiscovery {
            get {
                if (adviceNoneReferenceDiscovery == null) {
                    var diag = TestUtilities.QuietDiagnostics;
                    adviceNoneReferenceDiscovery = new ModuleReferenceDiscovery(new AdviceNoneAdvisor(diag), diag);
                }
                return adviceNoneReferenceDiscovery;
            }
        }

        public static ISchemaSerializer DefaultSchemaSerializer {
            get { return new JsonNETSchemaSerializer(new DefaultAdvicedContractResolver()); }
        }

        public static SchemaSerializationContext DefaultSchemaSerializationContext {
            get { return new EmbeddedResourceSchemaSerializationContext(
                DefaultSchemaSerializer, 
                EmbeddedResourceSchemaSerializationContext.DefaultResourceStreamName,
                QuietDiagnostics);
            }
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