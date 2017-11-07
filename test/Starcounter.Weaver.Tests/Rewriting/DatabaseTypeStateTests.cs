
using Starcounter.Weaver.Rewriting;
using System;
using System.Linq;
using Xunit;

namespace Starcounter.Weaver.Tests {

    public class DatabaseTypeStateTests {

        class TestClass {

        }

        [Fact]
        public void BadInputReportMeaningfulErrors() {
            var npe = Assert.Throws<ArgumentNullException>(() => new DatabaseTypeState(null));
            Assert.Equal("typeDefinition", npe.ParamName);

            npe = Assert.Throws<ArgumentNullException>(() => new DatabaseTypeStateEmitter(null));
            Assert.Equal("typeDefinition", npe.ParamName);
        }

        [Fact]
        public void SingleClassStateEmission() {
            var module = TestUtilities.GetModuleOfCurrentAssembly();

            var type = module.Types.Single(t => t.FullName == typeof(DatabaseTypeStateTests).FullName);
            Assert.NotNull(type);
            type = type.NestedTypes.First(t => t.Name == nameof(DatabaseTypeStateTests.TestClass));
            Assert.NotNull(type);

            var emitter = new DatabaseTypeStateEmitter(type);
            emitter.EmitReferenceFields();

            var state = (DatabaseTypeState) emitter;
            Assert.NotNull(state.DbId);
            Assert.NotNull(state.DbRef);

            emitter.EmitCRUDHandles();
            Assert.NotNull(state.CreateHandle);
            Assert.NotNull(state.DeleteHandle);

            emitter.EmitPropertyCRUDHandle("test");
            Assert.NotNull(state.GetPropertyHandle("test"));
        }
    }
}