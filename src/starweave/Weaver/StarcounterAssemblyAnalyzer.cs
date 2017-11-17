
using Mono.Cecil;
using Starcounter.Hosting.Schema;
using Starcounter.Weaver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace starweave.Weaver {

    public class StarcounterAssemblyAnalyzer : IAssemblyAnalyzer {
        readonly IWeaverHost host;
        readonly ModuleDefinition module;
        readonly IEnumerable<string> dataTypes;
        
        public StarcounterAssemblyAnalyzer(IWeaverHost weaverHost, ModuleDefinition moduleDefinition, IEnumerable<string> supportedDataTypes) {
            host = weaverHost ?? throw new ArgumentNullException(nameof(weaverHost));
            module = moduleDefinition ?? throw new ArgumentNullException(nameof(moduleDefinition));
            dataTypes = supportedDataTypes ?? throw new ArgumentNullException(nameof(supportedDataTypes));
        }
        
        bool IAssemblyAnalyzer.IsTargetReference(ModuleDefinition module) {
            return module.Name.Equals("Starcounter2.dll");
        }

        void IAssemblyAnalyzer.DiscoveryAssembly(DatabaseAssembly assembly) {
            foreach (var dataType in dataTypes) {
                assembly.DefiningSchema.DefineDataType(dataType);
            }
            
            var types = DiscoverDefinedDatabaseTypes();

            var databaseTypes = assembly.DefineTypes(
                types.Select(t => Tuple.Create(t.GetBindingName(), t.GetBaseTypeBindingName())).ToArray()
            );
            
            // After that, we have all types registered. Now we can
            // correctly discover and analyze all properties, knowing
            // the entire schema.

            foreach (var type in databaseTypes) {
                var typeDef = type.GetTypeDefinition(module);
                var props = DiscoverDefinedDatabaseProperties(typeDef);
                foreach (var p in props) {
                    type.DefineProperty(p.GetBindingName(), p.PropertyType.GetBindingName());
                }
            }
        }

        IEnumerable<TypeDefinition> DiscoverDefinedDatabaseTypes() {
            return module.Types.Where(t => t.HasCustomAttribute(typeof(Starcounter2.DatabaseAttribute)));
        }

        IEnumerable<PropertyDefinition> DiscoverDefinedDatabaseProperties(TypeDefinition type) {
            return type.Properties.Where(p => p.IsAutoImplemented());
        }
    }
}
