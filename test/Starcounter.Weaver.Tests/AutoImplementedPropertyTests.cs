
using System.Linq;
using Xunit;

namespace Starcounter.Weaver.Tests {

    public class AutoImplementedPropertyTests {

        [Fact]
        public void AllPropertiesOfIntAutoPropertiesIsAutoImplemented() {
            var module = TestUtilities.GetModuleOfCurrentAssembly();

            var type = module.Types.Single(t => t.FullName == typeof(IntAutoProperties).FullName);
            Assert.NotNull(type);

            Assert.True(type.Properties.All(p => p.IsAutoImplemented()));
        }   
    }
}