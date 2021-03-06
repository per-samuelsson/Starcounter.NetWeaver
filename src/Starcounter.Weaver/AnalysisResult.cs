﻿
using Mono.Cecil;
using Starcounter.Weaver.Runtime;

namespace Starcounter.Weaver {

    public sealed class AnalysisResult {

        public ModuleDefinition SourceModule { get; internal set; }

        public ModuleDefinition TargetModule { get; internal set; }

        public DatabaseAssembly AnalyzedAssembly { get; internal set; }

        public DatabaseSchema Schema {
            get {
                return AnalyzedAssembly.DefiningSchema;
            }
        }
    }
}