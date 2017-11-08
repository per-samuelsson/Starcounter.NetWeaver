
using Mono.Cecil;
using Starcounter.Weaver.Rewriting;
using Starcounter.Weaver.Tests.ExternalCode;
using System;
using System.Linq;
using Xunit;

namespace Starcounter.Weaver.Tests {

    public class DatabaseTypeStateTests {

        class TestClass {}

        class Derived : ClassWithExplicitStateReferenceFields { }

        class CustomStateNames : DatabaseTypeStateNames {
            public override string DbId => "dbId";
            public override string DbRef => "dbRef";
        }
        
        [Fact]
        public void BadInputReportMeaningfulErrors() {
            var names = new DatabaseTypeStateNames();
            var npe = Assert.Throws<ArgumentNullException>(() => new DatabaseTypeState(null, names));
            Assert.Equal("typeDefinition", npe.ParamName);

            npe = Assert.Throws<ArgumentNullException>(() => new DatabaseTypeStateEmitter(null, names));
            Assert.Equal("typeDefinition", npe.ParamName);
        }

        [Fact]
        public void SingleClassStateEmission() {

            using (var writeModule = TestUtilities.GetModuleOfCurrentAssemblyForRewriting()) {
                var module = writeModule.Module;

                var type = module.Types.Single(t => t.FullName == typeof(DatabaseTypeStateTests).FullName);
                Assert.NotNull(type);
                type = type.NestedTypes.First(t => t.Name == nameof(DatabaseTypeStateTests.TestClass));
                Assert.NotNull(type);

                var names = new DatabaseTypeStateNames();
                var emitter = new DatabaseTypeStateEmitter(type, names);
                emitter.EmitReferenceFields();

                var state = (DatabaseTypeState)emitter;
                Assert.NotNull(state.DbId);
                Assert.NotNull(state.DbRef);

                emitter.EmitCRUDHandles();
                Assert.NotNull(state.CreateHandle);
                Assert.NotNull(state.DeleteHandle);

                emitter.EmitPropertyCRUDHandle("test");
                Assert.NotNull(state.GetPropertyHandle("test"));
            }
        }

        [Fact]
        public void DerivedClassStateLookup() {

            using (var writeModule = TestUtilities.GetModuleOfCurrentAssemblyForRewriting()) {
                var module = writeModule.Module;

                var type = module.Types.Single(t => t.FullName == typeof(DatabaseTypeStateTests).FullName);
                Assert.NotNull(type);
                type = type.NestedTypes.First(t => t.Name == nameof(DatabaseTypeStateTests.Derived));
                Assert.NotNull(type);

                var names = new CustomStateNames();
                var state = new DatabaseTypeState(type, names);
                Assert.NotNull(state.DbId);
                Assert.NotNull(state.DbRef);
            }
        }
        
        [Fact]
        public void EmittingFullState() {

            using (var writeModule = TestUtilities.GetModuleOfCurrentAssemblyForRewriting()) {
                var module = writeModule.Module;

                var type = module.Types.Single(t => t.FullName == typeof(DatabaseTypeStateTests).FullName);
                Assert.NotNull(type);
                type = type.NestedTypes.First(t => t.Name == nameof(DatabaseTypeStateTests.TestClass));
                Assert.NotNull(type);

                var emitter = new DatabaseTypeStateEmitter(type, new DatabaseTypeStateNames());
                emitter.EmitReferenceFields();
                emitter.EmitCRUDHandles();
                emitter.EmitPropertyCRUDHandle("test");
                emitter.EmitPropertyCRUDHandle("test2");
                emitter.EmitPropertyCRUDHandle("test3");

                var state = new DatabaseTypeState(type, new DatabaseTypeStateNames());
                Assert.NotNull(state.DbId);
                Assert.NotNull(state.DbRef);
                Assert.NotNull(state.CreateHandle);
                Assert.NotNull(state.DeleteHandle);
                Assert.NotNull(state.GetPropertyHandle("test"));
                Assert.NotNull(state.GetPropertyHandle("test2"));
                Assert.NotNull(state.GetPropertyHandle("test3"));
            }
        }
    }
}