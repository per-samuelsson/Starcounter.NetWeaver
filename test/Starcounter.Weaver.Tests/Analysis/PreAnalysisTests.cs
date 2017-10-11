
using Starcounter.Hosting.Schema;
using Starcounter.Weaver.Analysis;
using Xunit;
using Mono.Cecil;

namespace Starcounter.Weaver.Tests {

    public class PreAnalysisTests {

        class CustomPreAnalysisNoSchemaFirstIsTarget : DefaultPreAnalysis {
            int count = 0;

            protected override DatabaseSchema DiscoverSchema(ModuleDefinition candidate, ISchemaSerializer serializer) {
                return null;
            }

            protected override bool IsTargetModule(ModuleDefinition candidate) {
                count++;
                return count == 1;
            }
        }

        [Fact]
        public void AdvicingNoneShouldRenderNoTarget() {
            var thisAssembly = TestUtilities.GetModuleOfCurrentAssembly();

            var preAnalysis = PreAnalysis.Execute<DefaultPreAnalysis>(
                thisAssembly,
                TestUtilities.AdviceNoneReferenceDiscovery,
                null,
                TestUtilities.QuietDiagnostics
            );
            Assert.NotNull(preAnalysis);
            Assert.Null(preAnalysis.TargetModule);
        }

        [Fact]
        public void CustomWithNullSerializerShouldBeAccepted() {
            var thisAssembly = TestUtilities.GetModuleOfCurrentAssembly();

            var preAnalysis = PreAnalysis.Execute<CustomPreAnalysisNoSchemaFirstIsTarget>(
                thisAssembly,
                TestUtilities.AdviceAllReferenceDiscovery,
                null,
                TestUtilities.QuietDiagnostics
            );
            Assert.NotNull(preAnalysis);
            Assert.NotNull(preAnalysis.TargetModule);
        }

        [Fact]
        public void ModuleWithReferencesWithoutSchemasShouldPass() {
            var thisAssembly = TestUtilities.GetModuleOfCurrentAssembly();

            var preAnalysis = PreAnalysis.Execute<DefaultPreAnalysis>(
                thisAssembly,
                TestUtilities.AdviceAllReferenceDiscovery,
                new JsonNETSchemaSerializer(),
                TestUtilities.QuietDiagnostics
            );
            Assert.NotNull(preAnalysis);
            Assert.NotNull(preAnalysis.TargetModule);
            Assert.Null(preAnalysis.ExternalSchema.Assemblies);
        }
    }
}
