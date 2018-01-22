using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SharedTestUtilities
{
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
}
