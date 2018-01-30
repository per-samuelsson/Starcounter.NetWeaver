
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Starcounter.Weaver.Runtime.Tests {

    class TestTypeStateNames : DatabaseTypeStateNames {
        readonly string createHandle = "createHandle";
        readonly string deleteHandle = "deleteHandle";

        public TestTypeStateNames(string customCreateHandle = null, string customDeleteHandle = null) {
            createHandle = customCreateHandle ?? createHandle;
            deleteHandle = customDeleteHandle ?? deleteHandle;
        }
        
        public override string CreateHandle => createHandle;

        public override string DeleteHandle => deleteHandle;

        public override string GetPropertyHandleName(string propertyName) {
            return "property_" + propertyName;
        }

        public static IEnumerable<string> GetAllPropertyHandleNames(Type type) {
            var prefix = "property_";
            return type.
                GetTypeInfo().
                DeclaredFields.
                Where(f => f.IsStatic && f.Name.StartsWith(prefix)).
                Select(f => f.Name.Substring(prefix.Length)
            );
        }
    }

    public class ClassWithCreateAndDeleteHandle {
        protected static ulong createHandle = 0;
        protected static ulong deleteHandle = 0;
    }

    public class ClassWithCreateAndDeleteHandlePlusProperties {
        protected static ulong createHandle = 0;
        protected static ulong deleteHandle = 0;

        protected static ulong property_Foo = 0;
        protected static ulong property_BarXXXXxxxxxxxxxxxxxxxxxxxxxxxxxxx = 0;
        protected static ulong property_asdfiiasdfkjalskdjflkjasdfasdfkljasdf1237237772347;
    }

    public class DefaultDatabaseTypeStateWriterTests {

        [Fact]
        public void NewWithNullTypeRaiseNullException() {
            Assert.Throws<ArgumentNullException>(() => new DefaultDatabaseTypeStateWriter(null, new DatabaseTypeStateNames()));
        }

        [Fact]
        public void NewWithNullNamesRaiseNullException() {
            Assert.Throws<ArgumentNullException>(() => new DefaultDatabaseTypeStateWriter(GetType(), null));
        }

        [Fact]
        public void NewWithMissingFieldRaiseMeaningfulException() {
            var type = typeof(ClassWithCreateAndDeleteHandle);

            var names = new TestTypeStateNames(customCreateHandle: "missing_create");
            var e = Assert.Throws<ArgumentException>(() => new DefaultDatabaseTypeStateWriter(type, names));
            Assert.Contains(type.Name, e.Message);
            Assert.Contains(names.CreateHandle, e.Message);

            names = new TestTypeStateNames(customDeleteHandle: "missing_delete");
            e = Assert.Throws<ArgumentException>(() => new DefaultDatabaseTypeStateWriter(type, names));
            Assert.Contains(type.Name, e.Message);
            Assert.Contains(names.DeleteHandle, e.Message);
        }

        [Fact]
        public void AccessingMissingPropertyHandleRaiseMeaningfulException() {
            var type = typeof(ClassWithCreateAndDeleteHandle);
            var writer = new DefaultDatabaseTypeStateWriter(type, new TestTypeStateNames());

            var propertyName = "property_not_defined";
            var e = Assert.Throws<ArgumentException>(() => writer.GetPropertyHandle(propertyName));
            Assert.Contains(type.Name, e.Message);
            Assert.Contains(propertyName, e.Message);

            e = Assert.Throws<ArgumentException>(() => writer.SetPropertyHandle(propertyName, 42));
            Assert.Contains(type.Name, e.Message);
            Assert.Contains(propertyName, e.Message);
        }

        [Theory]
        [InlineData(42)]
        [InlineData(1024)]
        [InlineData(ulong.MaxValue - 1087)]
        public void CreateHandleRoundTripRenderExpectedValue(ulong value) {
            var type = typeof(ClassWithCreateAndDeleteHandle);
            var writer = new DefaultDatabaseTypeStateWriter(type, new TestTypeStateNames());

            writer.CreateHandle = value;
            Assert.True(value == writer.CreateHandle);
        }

        [Theory]
        [InlineData(42)]
        [InlineData(1024 * 16)]
        [InlineData(ulong.MaxValue - 5099)]
        public void DeleteHandleRoundTripRenderExpectedValue(ulong value) {
            var type = typeof(ClassWithCreateAndDeleteHandle);
            var writer = new DefaultDatabaseTypeStateWriter(type, new TestTypeStateNames());

            writer.DeleteHandle = value;
            Assert.True(value == writer.DeleteHandle);
        }
        
        [Theory]
        [InlineData(42)]
        [InlineData(1024 * 16)]
        [InlineData(ulong.MaxValue - 5099)]
        [InlineData(ulong.MaxValue - 12876)]
        [InlineData(ulong.MinValue + 12098)]
        public void PropertyHandleRoundTripRenderExpectedValue(ulong value) {
            var type = typeof(ClassWithCreateAndDeleteHandlePlusProperties);
            var writer = new DefaultDatabaseTypeStateWriter(type, new TestTypeStateNames());

            foreach (var propertyName in TestTypeStateNames.GetAllPropertyHandleNames(type)) {
                writer.SetPropertyHandle(propertyName, value);
                Assert.True(value == writer.GetPropertyHandle(propertyName));
            }
        }
    }
}
