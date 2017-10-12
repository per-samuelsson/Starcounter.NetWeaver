
using Starcounter.Hosting.Schema;
using Starcounter.Weaver.Analysis;
using Xunit;
using Mono.Cecil;
using System.Linq;

namespace Starcounter.Weaver.Tests {

    public class PreAnalysisTests {

        class CustomPreAnalysisNoSchemaFirstIsTarget : DefaultPreAnalysis {
            int count = 0;

            public CustomPreAnalysisNoSchemaFirstIsTarget(
                ModuleReferenceDiscovery moduleReferenceDiscovery, 
                ISchemaSerializer schemaSerializer, 
                WeaverDiagnostics diagnostics) : base(moduleReferenceDiscovery, schemaSerializer, diagnostics) {
            }

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
            var preAnalyser = new DefaultPreAnalysis(TestUtilities.AdviceNoneReferenceDiscovery, new JsonNETSchemaSerializer(), TestUtilities.QuietDiagnostics);

            ModuleDefinition target;
            DatabaseSchema schema;
            preAnalyser.Execute(thisAssembly, out target, out schema);
            
            Assert.Null(target);
        }

        [Fact]
        public void CustomWithNullSerializerShouldBeAccepted() {
            var thisAssembly = TestUtilities.GetModuleOfCurrentAssembly();
            var preAnalyser = new CustomPreAnalysisNoSchemaFirstIsTarget(
                TestUtilities.AdviceAllReferenceDiscovery, 
                new JsonNETSchemaSerializer(),
                TestUtilities.QuietDiagnostics);

            ModuleDefinition target;
            DatabaseSchema schema;
            preAnalyser.Execute(thisAssembly, out target, out schema);
            
            Assert.NotNull(target);
        }

        [Fact]
        public void ModuleWithReferencesWithoutSchemasShouldPass() {
            var thisAssembly = TestUtilities.GetModuleOfCurrentAssembly();
            var preAnalyser = new DefaultPreAnalysis(
                TestUtilities.AdviceAllReferenceDiscovery, 
                new JsonNETSchemaSerializer(), 
                TestUtilities.QuietDiagnostics
            );

            ModuleDefinition target;
            DatabaseSchema schema;
            preAnalyser.Execute(thisAssembly, out target, out schema);

            Assert.NotNull(target);
            Assert.Equal(0, schema.Assemblies.Count());
        }
    }
}
