
using Starcounter.Weaver;
using starweave.Weaver;
using starweave.Weaver.Tests;
using System.Linq;
using Xunit;

namespace starweave.Tests {

    class EmitTarget {

    }

    class InsertConstructorParameterType {

    }

    class DbCrudFake {

        public static void Crete(out ulong dbId, out ulong dbRef, ulong createHandle) {
            dbId = 42;
            dbRef = 42;
        }
    }

    public class InsertConstructorEmitterTests {
        
        [Fact]
        public void EmitterAddConstructor() {
            using (var moduleHandle = TestUtilities.GetModuleOfCurrentAssemblyForRewriting()) {
                var module = moduleHandle.Module;
                
                var signature = module.Types.Single(t => t.FullName == typeof(InsertConstructorParameterType).FullName);
                var crud = module.Types.Single(t => t.FullName == typeof(DbCrudFake).FullName);
                var createMethod = crud.Methods.Single(m => m.Name == nameof(DbCrudFake.Crete));

                var emitter = new InsertConstructorEmitter(
                    new CodeEmissionContext(module), 
                    signature, 
                    createMethod
                );

                var testClass = module.Types.Single(t => t.FullName == typeof(EmitTarget).FullName);
                var ctors = testClass.Methods.Where(m => m.Name == ".ctor");
                Assert.Equal(1, ctors.Count());

                var state = new DatabaseTypeStateEmitter(new CodeEmissionContext(module), testClass, new DatabaseTypeStateNames());
                state.EmitCRUDHandles();
                state.EmitReferenceFields();

                emitter.Emit(testClass, state);
                ctors = testClass.Methods.Where(m => m.Name == ".ctor");
                Assert.Equal(2, ctors.Count());
            }
        }
    }
}
