using Starcounter.Weaver;
using starweave.Weaver;
using starweave.Weaver.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace starweave.Tests {

    public class ClassThatWillReceiveProxyImpl { }

    public interface IDbProxyStateWithOnlyGetId {

        ulong GetDbId();
    }

    public interface IDbProxyStateWithAllMethods {

        ulong GetDbId();

        void SetDbId(ulong id);

        ulong GetDbRef();

        void SetDbRef(ulong @ref);
    }

    public interface IDbProxyStateWithAllMethodsPlusIllegalExtra : IDbProxyStateWithAllMethods {
        int NotAllowed();
    }

    public class ClassImplementingIDbProxyState_Solution : IDbProxyStateWithAllMethods {
        protected ulong dbId;
        protected ulong dbRef;

        ulong IDbProxyStateWithAllMethods.GetDbId() {
            return dbId;
        }

        ulong IDbProxyStateWithAllMethods.GetDbRef() {
            return dbRef;
        }

        void IDbProxyStateWithAllMethods.SetDbId(ulong id) {
            dbId = id;
        }

        void IDbProxyStateWithAllMethods.SetDbRef(ulong @ref) {
            dbRef = @ref;
        }
    }

    public class DatabaseTypeIDbProxyStateEmitterTests {

        [Fact]
        public void SupportInterfaceWithOnlySingleMethod() {

            using (var mod = TestUtilities.GetModuleOfCurrentAssemblyForRewriting()) {
                var module = mod.Module;

                var emitContext = new CodeEmissionContext(module);
                var proxyInterface = module.Types.Single(t => t.FullName == typeof(IDbProxyStateWithOnlyGetId).FullName);
                var type = module.Types.Single(t => t.FullName == typeof(ClassThatWillReceiveProxyImpl).FullName);
                Assert.NotNull(type);

                var state = new DatabaseTypeStateEmitter(emitContext, type, new DatabaseTypeStateNames());
                state.EmitReferenceFields();

                var emitter = new DatabaseTypeIDbProxyStateEmitter(
                    emitContext, 
                    proxyInterface
                );

                Assert.Equal(1, type.Methods.Count); // 1 = ctor
                emitter.ImplementOn(type, state);
                Assert.Equal(2, type.Methods.Count);
            }
        }

        [Fact]
        public void SupportInterfaceWithAllMethods() {

            using (var mod = TestUtilities.GetModuleOfCurrentAssemblyForRewriting()) {
                var module = mod.Module;

                var emitContext = new CodeEmissionContext(module);
                var proxyInterface = module.Types.Single(t => t.FullName == typeof(IDbProxyStateWithAllMethods).FullName);
                var type = module.Types.Single(t => t.FullName == typeof(ClassThatWillReceiveProxyImpl).FullName);
                Assert.NotNull(type);

                var state = new DatabaseTypeStateEmitter(emitContext, type, new DatabaseTypeStateNames());
                state.EmitReferenceFields();

                var emitter = new DatabaseTypeIDbProxyStateEmitter(
                    emitContext,
                    proxyInterface
                );

                Assert.Equal(1, type.Methods.Count); // 1 = ctor
                emitter.ImplementOn(type, state);
                Assert.Equal(5, type.Methods.Count);
            }
        }

        [Fact]
        public void FailWhenProvidedUnknownInterfaceMethod() {

            using (var mod = TestUtilities.GetModuleOfCurrentAssemblyForRewriting()) {
                var module = mod.Module;

                var emitContext = new CodeEmissionContext(module);
                var proxyInterface = module.Types.Single(t => t.FullName == typeof(IDbProxyStateWithAllMethodsPlusIllegalExtra).FullName);
                var type = module.Types.Single(t => t.FullName == typeof(ClassThatWillReceiveProxyImpl).FullName);
                Assert.NotNull(type);

                var state = new DatabaseTypeStateEmitter(emitContext, type, new DatabaseTypeStateNames());
                state.EmitReferenceFields();

                Assert.Throws<ArgumentException>(() => {
                    new DatabaseTypeIDbProxyStateEmitter(emitContext, proxyInterface);
                });
            }
        }
    }
}
