using Starcounter.Weaver.Analysis;
using System;
using Xunit;

namespace Starcounter.Weaver.Tests {

    public class SingleModuleReferenceFinderTests {

        [Fact]
        public void MultipleHitsRaiseException() {
            var thisAssembly = TestUtilities.GetModuleOfCurrentAssembly();

            int count = 0;
            var e = Assert.Throws<Exception>(() => {
                SingleModuleReferenceFinder.Run(
                thisAssembly, TestUtilities.AdviceAllReferenceDiscovery, (module) => {
                    count++;
                    // Report each is a hit: should fail on second call.
                    return true;
                });
            });
            Assert.Equal(2, count);
            Assert.True(e.Message.Contains("Multiple results"));
        }


        [Fact]
        public void StarcounterAssemblyIsFound() {
            var thisAssembly = TestUtilities.GetModuleOfCurrentAssembly();

            var finder = SingleModuleReferenceFinder.Run(
                thisAssembly, TestUtilities.AdviceAllReferenceDiscovery, (module) => {
                    return module.Name.Equals("Starcounter.Weaver.dll");
                });

            Assert.NotNull(finder);
            Assert.NotNull(finder.Result);
        }
    }
}
