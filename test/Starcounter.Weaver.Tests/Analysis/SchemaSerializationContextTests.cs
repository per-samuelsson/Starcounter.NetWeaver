using Starcounter.Hosting.Schema;
using Starcounter.Weaver.Analysis;
using System;
using Xunit;

namespace Starcounter.Weaver.Tests {

    public class SchemaSerializationContextTests {

        [Fact]
        public void InvalidParametersTriggerMeaningfulErrors() {
            var thisAssembly = TestUtilities.GetModuleOfCurrentAssembly();
            var diagostics = TestUtilities.QuietDiagnostics;
            var dummySerializer = new DelegatingSchemaSerializer();

            var npe = Assert.Throws<ArgumentNullException>(() => new EmbeddedResourceSchemaSerializationContext(null, "test", diagostics));
            Assert.Contains("schemaSerializer", npe.Message);
            Assert.Equal("schemaSerializer", npe.ParamName);

            npe = Assert.Throws<ArgumentNullException>(() => new EmbeddedResourceSchemaSerializationContext(dummySerializer, null, diagostics));
            Assert.Contains("resourceName", npe.Message);
            Assert.Equal("resourceName", npe.ParamName);

            npe = Assert.Throws<ArgumentNullException>(() => new EmbeddedResourceSchemaSerializationContext(dummySerializer, string.Empty, diagostics));
            Assert.Contains("resourceName", npe.ParamName);
            Assert.Equal("resourceName", npe.ParamName);

            npe = Assert.Throws<ArgumentNullException>(() => new EmbeddedResourceSchemaSerializationContext(dummySerializer, "test", null));
            Assert.Contains("weaverDiagnostics", npe.Message);
            Assert.Equal("weaverDiagnostics", npe.ParamName);

            var context = new EmbeddedResourceSchemaSerializationContext(dummySerializer, "test", diagostics);
            Assert.NotNull(context);
            npe = Assert.Throws<ArgumentNullException>(() => context.Read(null));
            Assert.Contains("module", npe.Message);
            Assert.Equal("module", npe.ParamName);
            npe = Assert.Throws<ArgumentNullException>(() => context.Write(null, new DatabaseSchema()));
            Assert.Contains("module", npe.Message);
            Assert.Equal("module", npe.ParamName);
            npe = Assert.Throws<ArgumentNullException>(() => context.Write(thisAssembly, null));
            Assert.Contains("schema", npe.Message);
            Assert.Equal("schema", npe.ParamName);
        }
    }
}
