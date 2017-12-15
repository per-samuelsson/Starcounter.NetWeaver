using Mono.Cecil;
using Starcounter.Hosting.Schema;
using Starcounter.Hosting.Schema.Serialization;
using Starcounter.Weaver;
using Starcounter.Weaver.Analysis;
using System;
using System.IO;
using System.Reflection;
using Xunit;

using ModuleDefinition = Mono.Cecil.ModuleDefinition;

namespace starweave.Weaver.Tests {

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
            OutputStream = discardTestWrite ? null : new MemoryStream();
        }

        public Stream OutputStream {
            get; set;
        }

        public void Dispose() {
            if (!discardWrite) {
                module?.Write(OutputStream);
            }
            module = null;
        }
    }

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

        public static WeaverDiagnostics QuietDiagnostics {
            // Use this. We don't now if we want to keep WeaverDiagnostics.Quiet.
            get {
                if (quietDiagnostics == null) {
                    quietDiagnostics = WeaverDiagnostics.Quiet;
                }
                return quietDiagnostics;
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