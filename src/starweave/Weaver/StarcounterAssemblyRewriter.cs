
using Starcounter.Hosting.Schema;
using Starcounter.Weaver;
using Starcounter2.Internal;
using System;

namespace starweave.Weaver {

    public class StarcounterAssemblyRewriter : IAssemblyRewriter {
        readonly IWeaverHost host;
        readonly AnalysisResult analysis;
        readonly DatabaseTypeStateNames names;
        readonly DbCrudMethodProvider crudMethods;
        readonly CodeEmissionContext emitContext;

        public StarcounterAssemblyRewriter(IWeaverHost weaverHost, AnalysisResult result, DatabaseTypeStateNames stateNames, DbCrudMethodProvider methodProvider) {
            host = weaverHost ?? throw new ArgumentNullException(nameof(weaverHost));
            analysis = result ?? throw new ArgumentNullException(nameof(result));
            names = stateNames ?? throw new ArgumentNullException(nameof(stateNames));
            crudMethods = methodProvider ?? throw new ArgumentNullException(nameof(methodProvider));
            emitContext = new CodeEmissionContext(result.SourceModule);
        }
        
        void IAssemblyRewriter.RewriteType(DatabaseType type) {
            var module = analysis.SourceModule;

            var typeDef = type.GetTypeDefinition(module);

            var stateEmitter = new DatabaseTypeStateEmitter(emitContext, typeDef, names);
            stateEmitter.EmitCRUDHandles();
            stateEmitter.EmitReferenceFields();

            var propRewriter = new AutoImplementedPropertyRewriter(stateEmitter);

            foreach (var databaseProperty in type.Properties) {
                stateEmitter.EmitPropertyCRUDHandle(databaseProperty.Name);

                var propertyDef = databaseProperty.GetPropertyDefinition(typeDef);
                var autoProperty = new AutoImplementedProperty(propertyDef);

                // Here, we need to check the data type of the property: if it's
                // not a primitive, it's an enum or a reference. If so, we should
                // get method of "reference" / "object". The named data type must
                // support that interface. Also: there will need to be a cast in
                // the getter.
                // TODO:

                var readMethod = crudMethods.GetReadMethod(databaseProperty.DataType.Name);
                var writeMethod = crudMethods.GetUpdateMethod(databaseProperty.DataType.Name);

                var reader = emitContext.Use(readMethod);
                var writer = emitContext.Use(writeMethod);
                
                propRewriter.Rewrite(autoProperty, reader, writer);
            }
        }
    }
}