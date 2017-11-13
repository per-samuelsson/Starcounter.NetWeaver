
using Starcounter.Hosting.Schema;
using Starcounter.Weaver;
using Starcounter.Weaver.Rewriting;
using System;

namespace starweave.Weaver {

    public class StarcounterAssemblyRewriter : IAssemblyRewriter {
        readonly IWeaverHost host;
        readonly AnalysisResult analysis;
        readonly DatabaseTypeStateNames names;

        public StarcounterAssemblyRewriter(IWeaverHost weaverHost, AnalysisResult result, DatabaseTypeStateNames stateNames) {
            host = weaverHost ?? throw new ArgumentNullException(nameof(weaverHost));
            analysis = result ?? throw new ArgumentNullException(nameof(result));
            names = stateNames ?? throw new ArgumentNullException(nameof(stateNames));
        }

        void IAssemblyRewriter.RewriteType(DatabaseType type) {
            var typeDef = type.GetTypeDefinition(analysis.SourceModule);

            var stateEmitter = new DatabaseTypeStateEmitter(typeDef, names);
            stateEmitter.EmitCRUDHandles();
            stateEmitter.EmitReferenceFields();

            var propRewriter = new AutoImplementedPropertyRewriter(stateEmitter);

            foreach (var databaseProperty in type.Properties) {
                var propertyDef = databaseProperty.GetPropertyDefinition(typeDef);
                stateEmitter.EmitPropertyCRUDHandle(databaseProperty.Name);
                
                var autoProperty = new AutoImplementedProperty(propertyDef);
                
                // TODO:
                //propRewriter.Rewrite(autoProperty, null, null);
            }
        }
    }
}