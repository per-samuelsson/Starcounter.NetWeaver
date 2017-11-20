
using Xunit;

namespace Starcounter.Weaver.Tests {

    public class CecilExtensionMethodsTests {

        [Fact]
        public void CanResolveObjectConstructorMethodReference() {
            var module = TestUtilities.GetModuleOfCurrentAssembly();
            var oc = module.GetObjectConstructorReference();
            Assert.NotNull(oc);
        }
    }
}