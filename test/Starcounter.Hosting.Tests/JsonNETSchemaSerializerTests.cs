
using Starcounter.Hosting.Schema;
using Starcounter.Hosting.Schema.Serialization;
using System;
using System.Linq;
using Xunit;

namespace Starcounter.Hosting.Tests {

    public class JsonNETSchemaSerializerTests {

        static ISchemaSerializer CreateSerializer() {
            return new JsonNETSchemaSerializer(new DefaultAdvicedContractResolver());
        }

        [Fact]
        public void InvalidParametersTriggerMeaningfulErrors() {
            var serializer = CreateSerializer();

            var npe = Assert.Throws<ArgumentNullException>(() => serializer.Serialize(null));
            Assert.Contains("schema", npe.Message);
            Assert.Equal("schema", npe.ParamName);

            npe = Assert.Throws<ArgumentNullException>(() => serializer.Deserialize(null));
            Assert.Contains("schema", npe.Message);
            Assert.Equal("schema", npe.ParamName);
        }

        [Fact]
        public void RoundtripWithEmptySchema() {
            var serializer = CreateSerializer();
            var schema = new DatabaseSchema();
            var bytes = serializer.Serialize(schema);
            Assert.NotNull(bytes);
            Assert.NotEmpty(bytes);
            Assert.Empty(schema.DataTypes);
            var schema2 = serializer.Deserialize(bytes);
            Assert.NotNull(schema2);
            Assert.Empty(schema2.Assemblies);
            Assert.Empty(schema2.DataTypes);
        }

        [Fact]
        public void RoundtripWithOneDefinedAssembly() {
            var serializer = CreateSerializer();

            var schema = new DatabaseSchema();
            schema.DefineAssembly("test");
            Assert.Equal(1, schema.Assemblies.Count());
            Assert.Empty(schema.DataTypes);

            var bytes = serializer.Serialize(schema);
            Assert.NotNull(bytes);
            Assert.NotEmpty(bytes);

            var schema2 = serializer.Deserialize(bytes);
            Assert.NotNull(schema2);
            Assert.Equal(1, schema2.Assemblies.Count());
            Assert.Empty(schema2.DataTypes);

            var testAssembly = schema.Assemblies.Single();
            Assert.NotNull(testAssembly);
            Assert.Equal("test", testAssembly.Name);
            Assert.True(schema2.ContainSameAssemblies(testAssembly.DefiningSchema));
        }

        [Fact]
        public void CanRoundtripWithCustomDefinedTypes() {
            var serializer = CreateSerializer();
            
            var schema = new DatabaseSchema();
            var names = new[] {
                "test",
                "test.foo",
                "test.bar",
                "foo.bar.test.whatever"
            };

            foreach (var name in names) {
                schema.DefineDataType(name);
            }

            Assert.True(schema.Assemblies.Count() == 0);
            Assert.True(schema.DataTypes.Count() == names.Count());
            foreach (var name in names) {
                Assert.True(schema.DataTypes.Any(t => t.Name == name));
            }
            
            var bytes = serializer.Serialize(schema);
            Assert.NotNull(bytes);
            Assert.NotEmpty(bytes);

            var schema2 = serializer.Deserialize(bytes);
            Assert.NotNull(schema2);
            Assert.Empty(schema2.Assemblies);
            Assert.Equal(names.Count(), schema2.DataTypes.Count());

            foreach (var name in names) {
                Assert.True(schema2.DataTypes.Any(t => t.Name == name));
            }
        }

        [Fact]
        public void RoundtripWithOneDefinedType() {
            var serializer = CreateSerializer();

            var schema = new DatabaseSchema();
            var assembly = schema.DefineAssembly("test");
            var types = assembly.DefineTypes(
                new[] { Tuple.Create("test.test", typeof(System.Object).FullName) }
                );
            Assert.Equal(1, schema.Assemblies.Count());
            Assert.Equal(1, types.Count());
            Assert.Equal("test.test", types.Single().FullName);
            Assert.Equal(typeof(object).FullName, types.Single().BaseTypeName);
            Assert.Empty(types.Single().Properties);

            var bytes = serializer.Serialize(schema);
            Assert.NotNull(bytes);
            Assert.NotEmpty(bytes);

            var schema2 = serializer.Deserialize(bytes);
            Assert.NotNull(schema2);
            Assert.Equal(1, schema2.Assemblies.Count());

            var testAssembly = schema2.Assemblies.Single();
            Assert.NotNull(testAssembly);
            Assert.Equal("test", testAssembly.Name);
            Assert.True(schema2.ContainSameAssemblies(testAssembly.DefiningSchema));

            var testType = testAssembly.Types.Single();
            Assert.NotNull(testType);
            Assert.Equal("test.test", testType.FullName);
            Assert.Empty(testType.Properties);
            Assert.Equal(testAssembly, testType.DefiningAssembly);
        }
    }
}