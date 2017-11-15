
using Starcounter.Hosting.Schema;
using System;
using System.Linq;
using Xunit;

namespace Starcounter.Hosting.Tests {
    
    public class SchemaTests {

        [Fact]
        public void NewInstanceContainNoAssembliesOrTypes() {
            var schema = new DatabaseSchema();
            Assert.True(schema.Assemblies.Count() == 0);
            Assert.True(schema.Types.Count() == 0);
        }

        [Fact]
        public void DefineAssemblyWithBadInputReportMeaningfulErrors() {
            var schema = new DatabaseSchema();

            var npe = Assert.Throws<ArgumentNullException>(() => schema.DefineAssembly(null));
            Assert.Equal("name", npe.ParamName);

            npe = Assert.Throws<ArgumentNullException>(() => schema.DefineAssembly(string.Empty));
            Assert.Equal("name", npe.ParamName);

            Assert.True(schema.Assemblies.Count() == 0);
            Assert.True(schema.Types.Count() == 0);
        }

        [Fact]
        public void DefineAssembliesWithSameNameFail() {
            var schema = new DatabaseSchema();

            schema.DefineAssembly("test");
            Assert.Throws<ArgumentException>(() => schema.DefineAssembly("test"));
            
            Assert.True(schema.Assemblies.Count() == 1);
            Assert.True(schema.Types.Count() == 0);
        }

        [Fact]
        public void NamedDataTypesCanBeDefined() {
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
        }

        [Fact]
        public void TypesNotDefinedCantBeReferenced() {
            var schema = new DatabaseSchema();

            var assembly = schema.DefineAssembly("test");
            var rangeException = Assert.Throws<ArgumentOutOfRangeException>(() => {
                assembly.DefineTypes(new[] { Tuple.Create("test", "doesnotexist") });
                });
            Assert.Contains("doesnotexist", rangeException.Message);

            var type = assembly.DefineTypes(new [] { Tuple.Create<string, string>("test", null) }).First();
            Assert.NotNull(type);

            rangeException = Assert.Throws<ArgumentOutOfRangeException>(() => type.DefineProperty("name", "doesnotexist"));
            Assert.Contains("doesnotexist", rangeException.Message);
        }
    }
}