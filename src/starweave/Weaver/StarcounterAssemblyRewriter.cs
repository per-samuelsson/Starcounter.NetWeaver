using Starcounter.Hosting.Schema;
using Starcounter.Weaver;
using Starcounter.Weaver.Rewriting;
using System;

namespace starweave.Weaver {

    public class StarcounterAssemblyRewriter : IAssemblyRewriter {
        readonly IWeaverHost host;
        AnalysisResult analysis;

        public StarcounterAssemblyRewriter(IWeaverHost weaverHost, AnalysisResult result) {
            host = weaverHost ?? throw new ArgumentNullException(nameof(weaverHost));
            analysis = result ?? throw new ArgumentNullException(nameof(result));
        }

        void IAssemblyRewriter.RewriteType(DatabaseType type) {
            var stateEmitter = new DatabaseTypeStateEmitter(null, null);
            stateEmitter.EmitCRUDHandles();
            stateEmitter.EmitReferenceFields();
        }
    }
}
