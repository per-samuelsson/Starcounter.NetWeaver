
using Starcounter.Hosting.Schema;
using Starcounter.Weaver;
using Starcounter.Weaver.Rewriting;
using Starcounter2.Internal.WeaverFacade;
using System;

namespace starweave.Weaver {

    public class StarcounterAssemblyRewriter : IAssemblyRewriter {
        readonly IWeaverHost host;
        readonly AnalysisResult analysis;
        readonly DatabaseTypeStateNames names;
        readonly CRUDMethodProvider crudMethods;

        public StarcounterAssemblyRewriter(IWeaverHost weaverHost, AnalysisResult result, DatabaseTypeStateNames stateNames, CRUDMethodProvider methodProvider) {
            host = weaverHost ?? throw new ArgumentNullException(nameof(weaverHost));
            analysis = result ?? throw new ArgumentNullException(nameof(result));
            names = stateNames ?? throw new ArgumentNullException(nameof(stateNames));
            crudMethods = methodProvider ?? throw new ArgumentNullException(nameof(methodProvider));
        }

        void IAssemblyRewriter.RewriteType(DatabaseType type) {
            var module = analysis.SourceModule;

            var typeDef = type.GetTypeDefinition(module);

            var stateEmitter = new DatabaseTypeStateEmitter(typeDef, names);
            stateEmitter.EmitCRUDHandles();
            stateEmitter.EmitReferenceFields();

            var propRewriter = new AutoImplementedPropertyRewriter(stateEmitter);

            foreach (var databaseProperty in type.Properties) {
                stateEmitter.EmitPropertyCRUDHandle(databaseProperty.Name);

                var propertyDef = databaseProperty.GetPropertyDefinition(typeDef);
                var autoProperty = new AutoImplementedProperty(propertyDef);

                var readMethod = crudMethods.GetReadMethod(databaseProperty.DataType.Name);
                var writeMethod = crudMethods.GetUpdateMethod(databaseProperty.DataType.Name);

                var reader = module.ImportReference(readMethod);
                var writer = module.ImportReference(writeMethod);
                
                propRewriter.Rewrite(autoProperty, reader, writer);
            }
        }
    }
}