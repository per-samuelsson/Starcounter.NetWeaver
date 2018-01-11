
using Starcounter.Weaver.Runtime;
using Starcounter.Weaver.Runtime.Abstractions;
using System;
using System.Linq;
using Xunit;

namespace Starcounter.Weaver.Runtime.Tests {

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
        public void CanRoundtripWithEmptySchema() {
            var serializer = CreateSerializer();
            var schema = new DatabaseSchema();
            var bytes = serializer.Serialize(schema);
            Assert.NotNull(bytes);
            Assert.NotEmpty(bytes);
            Assert.Empty(schema.Types);
            var schema2 = serializer.Deserialize(bytes);
            Assert.NotNull(schema2);
            Assert.Empty(schema2.Assemblies);
            Assert.Empty(schema2.Types);
        }

        [Fact]
        public void CanRoundtripWithOneDefinedAssembly() {
            var serializer = CreateSerializer();

            var schema = new DatabaseSchema();
            schema.DefineAssembly("test");
            Assert.Equal(1, schema.Assemblies.Count());
            Assert.Empty(schema.Types);

            var bytes = serializer.Serialize(schema);
            Assert.NotNull(bytes);
            Assert.NotEmpty(bytes);

            var schema2 = serializer.Deserialize(bytes);
            Assert.NotNull(schema2);
            Assert.Equal(1, schema2.Assemblies.Count());
            Assert.Empty(schema2.Types);

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
            Assert.True(schema.Types.Count() == names.Count());
            foreach (var name in names) {
                Assert.True(schema.Types.Any(t => t.Name == name));
            }

            var bytes = serializer.Serialize(schema);
            Assert.NotNull(bytes);
            Assert.NotEmpty(bytes);

            var schema2 = serializer.Deserialize(bytes);
            Assert.NotNull(schema2);
            Assert.Empty(schema2.Assemblies);
            Assert.Equal(names.Count(), schema2.Types.Count());

            foreach (var name in names) {
                Assert.True(schema2.Types.Any(t => t.Name == name));
            }
        }

        [Fact]
        public void CanRoundtripWithOneDefinedType() {
            var serializer = CreateSerializer();

            var schema = new DatabaseSchema();
            var assembly = schema.DefineAssembly("test");
            var types = assembly.DefineTypes(
                new[] { Tuple.Create<string, string>("test.test", null) }
                );
            Assert.Equal(1, schema.Assemblies.Count());
            Assert.Equal(1, types.Count());
            Assert.Equal("test.test", types.Single().FullName);
            Assert.Null(types.Single().BaseTypeName);
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

        [Fact]
        public void CanRoundtripWithTwoDefinedTypesWithProperties() {
            var serializer = CreateSerializer();

            var schema = new DatabaseSchema();
            schema.DefineDataType("System.Int32");
            schema.DefineDataType("System.String");

            var assembly = schema.DefineAssembly("test");
            var types = assembly.DefineTypes(new[] {
                Tuple.Create<string, string>("test.test", null),
                Tuple.Create("test2.test2", "test.test")
            });
            Assert.Equal(1, schema.Assemblies.Count());
            Assert.Equal(2, types.Count());
            
            var type1 = types.First(t => t.FullName == "test.test");
            Assert.NotNull(type1);
            var type2 = types.First(t => t.FullName == "test2.test2" && t.BaseTypeName == "test.test");
            Assert.NotNull(type2);

            var property = type1.DefineProperty("test", "System.Int32");
            Assert.NotNull(property);
            Assert.Equal(property.Name, "test");
            Assert.Equal(property.DataType.Name, "System.Int32");

            property = type1.DefineProperty("test2", "System.String");
            Assert.NotNull(property);
            Assert.Equal(property.Name, "test2");
            Assert.Equal(property.DataType.Name, "System.String");

            property = type2.DefineProperty("test", type1.FullName);
            Assert.NotNull(property);
            Assert.Equal(property.Name, "test");
            Assert.Equal(property.DataType.Name, type1.FullName);

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
            Assert.Equal(2, testAssembly.Types.Count());

            type1 = types.First(t => t.FullName == "test.test");
            Assert.NotNull(type1);
            type2 = types.First(t => t.FullName == "test2.test2" && t.BaseTypeName == "test.test");
            Assert.NotNull(type2);

            Assert.Equal(2, type1.Properties.Count());
            property = type1.Properties.First(p => p.Name.Equals("test"));
            Assert.NotNull(property);
            Assert.Equal(property.DataType.Name, "System.Int32");

            property = type1.Properties.First(p => p.Name.Equals("test2"));
            Assert.NotNull(property);
            Assert.Equal(property.DataType.Name, "System.String");

            property = type2.Properties.First(p => p.Name.Equals("test"));
            Assert.NotNull(property);
            Assert.Equal(property.DataType.Name, type1.FullName);
        }
    }
}