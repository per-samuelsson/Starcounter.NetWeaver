
using Starcounter.Weaver.Rewriting;
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

        [Fact]
        public void AllPropertiesOfIntAutoPropertiesHasExpectedImplementation() {
            var module = TestUtilities.GetModuleOfCurrentAssembly();

            var type = module.Types.Single(t => t.FullName == typeof(IntAutoProperties).FullName);
            Assert.NotNull(type);

            Assert.True(type.Properties.All(p => {
                RewritingAssertionMethods.VerifyExpectedOriginalGetter(p.GetMethod);
                if (p.SetMethod != null) {
                    RewritingAssertionMethods.VerifyExpectedOriginalSetter(p.SetMethod);
                }
                return true;
            }));
        }
    }
}