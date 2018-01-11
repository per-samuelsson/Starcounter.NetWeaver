using Mono.Cecil;
using Starcounter.Weaver.Runtime;
using Starcounter.Weaver.Runtime.Abstractions;
using Starcounter.Weaver.Analysis;
using System;
using System.IO;
using System.Reflection;
using Xunit;

using ModuleDefinition = Mono.Cecil.ModuleDefinition;

namespace Starcounter.Weaver.Tests {
    
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
            currentAssemblyDefaultReaderParameters.ReadSymbols = false;
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
    }
}