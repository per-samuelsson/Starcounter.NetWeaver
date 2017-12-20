
using Starcounter.Hosting.Schema;
using Starcounter.Weaver.Analysis;
using Xunit;
using Mono.Cecil;
using System.Linq;

namespace Starcounter.Weaver.Tests {

    public class PreAnalysisTests {

        class AnalyzerThatReturnFirstModuleAsReference : IAssemblyAnalyzer {
            int count = 0;

            public void DiscoveryAssembly(AnalysisResult analysisResult) {
            }

            public bool IsTargetReference(ModuleDefinition module) {
                count++;
                return count == 1;
            }
        }

        class CustomPreAnalysisNoSchemaFirstIsTarget : DefaultPreAnalysis {

            public CustomPreAnalysisNoSchemaFirstIsTarget(
                ModuleReferenceDiscovery moduleReferenceDiscovery,
                SchemaSerializationContext serializationContext, 
                WeaverDiagnostics diagnostics) : base(moduleReferenceDiscovery, serializationContext, diagnostics) {
            }

            protected override DatabaseSchema DiscoverSchema(ModuleDefinition candidate, SchemaSerializationContext serializationContext) {
                return null;
            }
        }

        [Fact]
        public void AdvicingNoneShouldRenderNoTarget() {
            var thisAssembly = TestUtilities.GetModuleOfCurrentAssembly();
            var preAnalyser = new DefaultPreAnalysis(
                TestUtilities.AdviceNoneReferenceDiscovery, 
                TestUtilities.DefaultSchemaSerializationContext, 
                TestUtilities.QuietDiagnostics);
            
            preAnalyser.Execute(thisAssembly, new AnalyzerThatReturnFirstModuleAsReference(), out ModuleDefinition target, out DatabaseSchema schema);
            
            Assert.Null(target);
        }

        [Fact]
        public void CustomWithNullSerializerShouldBeAccepted() {
            var thisAssembly = TestUtilities.GetModuleOfCurrentAssembly();
            var preAnalyser = new CustomPreAnalysisNoSchemaFirstIsTarget(
                TestUtilities.AdviceAllReferenceDiscovery, 
                TestUtilities.DefaultSchemaSerializationContext,
                TestUtilities.QuietDiagnostics);
            
            preAnalyser.Execute(thisAssembly, new AnalyzerThatReturnFirstModuleAsReference(), out ModuleDefinition target, out DatabaseSchema schema);
            
            Assert.NotNull(target);
        }

        [Fact]
        public void ModuleWithReferencesWithoutSchemasShouldPass() {
            var thisAssembly = TestUtilities.GetModuleOfCurrentAssembly();
            var preAnalyser = new DefaultPreAnalysis(
                TestUtilities.AdviceAllReferenceDiscovery, 
                TestUtilities.DefaultSchemaSerializationContext, 
                TestUtilities.QuietDiagnostics
            );
            
            preAnalyser.Execute(thisAssembly, new AnalyzerThatReturnFirstModuleAsReference(), out ModuleDefinition target, out DatabaseSchema schema);

            Assert.NotNull(target);
            Assert.Equal(0, schema.Assemblies.Count());
        }
    }
}
