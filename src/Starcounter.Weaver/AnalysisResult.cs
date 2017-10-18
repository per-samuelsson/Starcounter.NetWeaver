
using Mono.Cecil;
using Starcounter.Hosting.Schema;

namespace Starcounter.Weaver {

    public sealed class AnalysisResult {

        public ModuleDefinition SourceModule { get; internal set; }

        public ModuleDefinition TargetModule { get; internal set; }

        public DatabaseAssembly AnalyzedAssembly { get; internal set; }
    }
}