
using Starcounter.Weaver.Analysis;
using System;
using Xunit;
using Mono.Cecil;

namespace Starcounter.Weaver.Tests {

    public class ModuleReferenceDiscoveryTests {
        
        [Fact]
        public void AdvisoryAdvicingNoneYieldZeroCalls() {
            var module = TestUtilities.GetModuleOfCurrentAssembly();
            var diag = WeaverDiagnostics.Quiet;
            var discovery = new ModuleReferenceDiscovery(module, new AdviceNoneAdvisor(diag), diag);

            int count = 0;
            discovery.DiscoverReferences((m) => {
                count++;
                return true;
            });

            Assert.Equal(0, count);
        }

        [Fact]
        public void StopWhenCallbackInstructToDoSo() {
            var module = TestUtilities.GetModuleOfCurrentAssembly();
            var diag = WeaverDiagnostics.Quiet;
            var discovery = new ModuleReferenceDiscovery(module, new AdviceAllAdvisor(diag), diag);

            int count = 0;
            discovery.DiscoverReferences((m) => {
                count++;
                return false;
            });
            Assert.Equal(1, count);

            count = 0;
            discovery.DiscoverReferences((m) => {
                count++;
                return count == 3 ? false : true;
            });
            Assert.Equal(3, count);
        }

        [Fact]
        public void FindXUnitReferenceAndStopWhenDoingSo() {
            var module = TestUtilities.GetModuleOfCurrentAssembly();
            var diag = WeaverDiagnostics.Quiet;
            var discovery = new ModuleReferenceDiscovery(module, new AdviceAllAdvisor(diag), diag);

            int count = 0;
            int countWhenXUnitIsFound = 0;
            discovery.DiscoverReferences((m) => {
                count++;
                if (m.Name.Equals("xunit.core.dll", StringComparison.InvariantCultureIgnoreCase)) {
                    countWhenXUnitIsFound = count;
                    return false;
                }
                
                return true;
            });
            Assert.NotEqual(0, countWhenXUnitIsFound);
            Assert.Equal(count, countWhenXUnitIsFound);
        }
    }
}