
using System;
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
    }

    public class ClassWithCreateAndDeleteHandle {
        protected static ulong createHandle = 0;
        protected static ulong deleteHandle = 0;
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
    }
}
