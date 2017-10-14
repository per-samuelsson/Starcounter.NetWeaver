
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
    }
}
