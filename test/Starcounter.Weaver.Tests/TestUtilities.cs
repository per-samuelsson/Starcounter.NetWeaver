using Mono.Cecil;
using Starcounter.Weaver.Runtime.JsonSerializer;
using Starcounter.Weaver.Runtime.Abstractions;
using Starcounter.Weaver.Analysis;
using System;
using System.IO;
using System.Reflection;
using Xunit;

using ModuleDefinition = Mono.Cecil.ModuleDefinition;
using SharedTestUtilities;

namespace Starcounter.Weaver.Tests {
    
    public static class TestUtilities {
        static byte[] currentAssemblyBytes;
        static ModuleDefinition currentAssemblyModule;
        static ReaderParameters currentAssemblyDefaultReaderParameters;
        static ModuleReferenceDiscovery adviceAllReferenceDiscovery;
        static ModuleReferenceDiscovery adviceNoneReferenceDiscovery;

        static TestUtilities() {
            var currentAssemblyPath = Assembly.GetExecutingAssembly().Location;
            currentAssemblyDefaultReaderParameters = new DefaultModuleReaderParameters(currentAssemblyPath).Parameters;
            currentAssemblyDefaultReaderParameters.ReadSymbols = false;
            currentAssemblyBytes = File.ReadAllBytes(currentAssemblyPath);
        }
        
        public static ModuleReferenceDiscovery AdviceAllReferenceDiscovery {
            get {
                if (adviceAllReferenceDiscovery == null) {
                    var diag = SharedTesting.QuietDiagnostics;
                    adviceAllReferenceDiscovery = new ModuleReferenceDiscovery(new AdviceAllAdvisor(diag), diag);
                }
                return adviceAllReferenceDiscovery;
            }
        }

        public static ModuleReferenceDiscovery AdviceNoneReferenceDiscovery {
            get {
                if (adviceNoneReferenceDiscovery == null) {
                    var diag = SharedTesting.QuietDiagnostics;
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
                SharedTesting.QuietDiagnostics);
            }
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