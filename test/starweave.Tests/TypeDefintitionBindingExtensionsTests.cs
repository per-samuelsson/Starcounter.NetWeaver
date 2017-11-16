
using System.Linq;
using Xunit;

namespace starweave.Weaver.Tests {

    public class ClassWithNoDefinedBaseClass {

    }

    public class TypeDefintitionBindingExtensionsTests {

        [Fact]
        public void TypesDerivingObjectShouldBindWithExpectedBaseName() {
            var module = TestUtilities.GetModuleOfCurrentAssembly();

            var type = module.Types.Single(t => t.FullName == typeof(ClassWithNoDefinedBaseClass).FullName);
            Assert.NotNull(type);

            Assert.Equal(TypeDefintitionBindingExtensions.BindingNameOfNoDeclaredBaseType, type.GetBaseTypeBindingName());
        }
    }
}