
using Mono.Cecil;
using Starcounter.Hosting.Schema;
using Starcounter.Weaver.Analysis;
using System.Collections.Generic;

namespace Starcounter.Weaver {

    public class ModuleWeaver {
        readonly SchemaSerializationContext schemaSerializationContext;
        
        public ModuleWeaver(SchemaSerializationContext serializationContext) {
            Guard.NotNull(serializationContext, nameof(serializationContext));

            schemaSerializationContext = serializationContext;
        }

        public ModuleDefinition Weave(AnalysisResult analysis, IAssemblyRewriter rewriter) {
            Guard.NotNull(analysis, nameof(analysis));
            Guard.NotNull(rewriter, nameof(rewriter));

            var assembly = analysis.AnalyzedAssembly;
            var module = analysis.SourceModule;

            var weavedTypes = new List<DatabaseType>();

            foreach (var type in assembly.Types) {
                WeaveType(rewriter, type, weavedTypes);
            }

            schemaSerializationContext.Write(module, assembly.DefiningSchema);
            return module;
        }

        void WeaveType(IAssemblyRewriter rewriter, DatabaseType type, List<DatabaseType> weavedTypes) {
            if (weavedTypes.Contains(type)) {
                return;
            }
            
            var baseType = type.GetBaseType();
            if (baseType != null && baseType.IsDefinedIn(type.DefiningAssembly)) {
                WeaveType(rewriter, type, weavedTypes);
            }

            weavedTypes.Add(type);
            rewriter.RewriteType(type);
        }
    }
}