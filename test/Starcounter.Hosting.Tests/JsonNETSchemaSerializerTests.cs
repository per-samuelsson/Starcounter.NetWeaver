
using Starcounter.Hosting.Schema;
using System;
using Xunit;

namespace Starcounter.Hosting.Tests {

    public class JsonNETSchemaSerializerTests {

        [Fact]
        public void InvalidParametersTriggerMeaningfulErrors() {
            var serializer = new JsonNETSchemaSerializer() as ISchemaSerializer;

            var npe = Assert.Throws<ArgumentNullException>(() => serializer.Serialize(null));
            Assert.Contains("schema", npe.Message);
            Assert.Equal("schema", npe.ParamName);

            npe = Assert.Throws<ArgumentNullException>(() => serializer.Deserialize(null));
            Assert.Contains("schema", npe.Message);
            Assert.Equal("schema", npe.ParamName);
        }

        [Fact]
        public void RoundtripWithEmptySchema() {
            var serializer = new JsonNETSchemaSerializer() as ISchemaSerializer;
            var schema = new DatabaseSchema();
            var bytes = serializer.Serialize(schema);
            Assert.NotNull(bytes);
            Assert.NotEmpty(bytes);
            var schema2 = serializer.Deserialize(bytes);
            Assert.NotNull(schema2);
            Assert.Empty(schema2.Assemblies);
        }
    }
}
