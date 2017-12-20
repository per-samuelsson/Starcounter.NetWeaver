
using Mono.Cecil;
using Starcounter.Hosting.Schema;
using Starcounter.Weaver.Analysis;

namespace Starcounter.Weaver {
    
    public class ModuleAnalyzer {
        readonly PreAnalysis preAnalysis;
        readonly WeaverDiagnostics diag;
        readonly DatabaseTypeDiscoveryProvider databaseDiscoveryProvider;
        
        public ModuleAnalyzer(
            PreAnalysis preAnalyser,
            DatabaseTypeDiscoveryProvider databaseTypeDiscovery,
            WeaverDiagnostics diagnostics) {

            Guard.NotNull(preAnalyser, nameof(preAnalyser));
            Guard.NotNull(databaseTypeDiscovery, nameof(databaseTypeDiscovery));
            Guard.NotNull(diagnostics, nameof(diagnostics));

            preAnalysis = preAnalyser;
            diag = diagnostics;
            databaseDiscoveryProvider = databaseTypeDiscovery;
        }
        
        public AnalysisResult Analyze(ModuleDefinition module, IAssemblyAnalyzer assemblyAnalyzer) {
            preAnalysis.Execute(
                module, 
                assemblyAnalyzer, 
                out ModuleDefinition targetReference, 
                out DatabaseSchema schema
            );

            var databaseDiscovery = databaseDiscoveryProvider.ProvideDiscovery(module, assemblyAnalyzer, targetReference, schema);
            if (databaseDiscovery == null) {
                return null;
            }

            var assembly = schema.DefineAssembly(module.Name);
            var result = new AnalysisResult() {
                SourceModule = module,
                TargetModule = targetReference,
                AnalyzedAssembly = assembly
            };

            databaseDiscovery.DiscoverAssembly(result);

            return result;
        }
    }
}