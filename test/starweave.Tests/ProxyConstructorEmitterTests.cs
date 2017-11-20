
using Starcounter.Weaver;
using starweave.Weaver;
using starweave.Weaver.Tests;
using System.Linq;
using Xunit;

namespace starweave.Tests {
    
    class ProxyConstructorParameterType {

    }

    class ProxyEmitTargetStandalone {

    }

    class ProxyEmitTargetBaseSameAssembly {

    }

    class ProxyEmitTargetExtendingSameAssembly : ProxyEmitTargetBaseSameAssembly {

    }


    public class ProxyConstructorEmitterTests {

        [Fact]
        public void EmittingStandaloneAddOneConstructor() {
            using (var moduleHandle = TestUtilities.GetModuleOfCurrentAssemblyForRewriting()) {
                var module = moduleHandle.Module;

                var signature = module.Types.Single(t => t.FullName == typeof(ProxyConstructorParameterType).FullName);

                var emitter = new ProxyConstructorEmitter(
                    new CodeEmissionContext(module),
                    signature
                );

                var testClass = module.Types.Single(t => t.FullName == typeof(ProxyEmitTargetStandalone).FullName);
                var ctors = testClass.Methods.Where(m => m.Name == ".ctor");
                Assert.Equal(1, ctors.Count());

                emitter.Emit(testClass);
                ctors = testClass.Methods.Where(m => m.Name == ".ctor");
                Assert.Equal(2, ctors.Count());
            }
        }

        [Fact]
        public void EmittingBaseAndDerivedInSameAssemblyAddConstructorPair() {
            using (var moduleHandle = TestUtilities.GetModuleOfCurrentAssemblyForRewriting()) {
                var module = moduleHandle.Module;

                var signature = module.Types.Single(t => t.FullName == typeof(ProxyConstructorParameterType).FullName);

                var emitter = new ProxyConstructorEmitter(
                    new CodeEmissionContext(module),
                    signature
                );

                var testClassBase = module.Types.Single(t => t.FullName == typeof(ProxyEmitTargetBaseSameAssembly).FullName);
                var ctors = testClassBase.Methods.Where(m => m.Name == ".ctor");
                Assert.Equal(1, ctors.Count());

                emitter.Emit(testClassBase);
                ctors = testClassBase.Methods.Where(m => m.Name == ".ctor");
                Assert.Equal(2, ctors.Count());

                var testClassDerived = module.Types.Single(t => t.FullName == typeof(ProxyEmitTargetExtendingSameAssembly).FullName);
                ctors = testClassDerived.Methods.Where(m => m.Name == ".ctor");
                Assert.Equal(1, ctors.Count());

                var baseConstructor = testClassBase.Methods.Single(m => m.Name == ".ctor" && m.Parameters.Count == 1);
                emitter.Emit(testClassDerived, baseConstructor);
                ctors = testClassDerived.Methods.Where(m => m.Name == ".ctor");
                Assert.Equal(2, ctors.Count());
            }
        }
    }
}