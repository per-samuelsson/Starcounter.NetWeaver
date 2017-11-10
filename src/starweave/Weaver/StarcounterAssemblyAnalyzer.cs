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


        public StarcounterAssemblyAnalyzer(IWeaverHost weaverHost, ModuleDefinition moduleDefinition) {
            host = weaverHost ?? throw new ArgumentNullException(nameof(weaverHost));
            module = moduleDefinition ?? throw new ArgumentNullException(nameof(moduleDefinition));
        }

        IEnumerable<TypeDefinition> DiscoverDefinedTypes() {
            return module.Types.Where(t => t.HasCustomAttribute(typeof(Starcounter2.DatabaseAttribute)));
        }

        IEnumerable<PropertyDefinition> DiscoverDefinedProperties(TypeDefinition type) {
            return type.Properties.Where(p => p.IsAutoImplemented());
        }

        bool IAssemblyAnalyzer.IsTargetReference(ModuleDefinition module) {
            return module.Name.Equals("Starcounter2");
        }

        void IAssemblyAnalyzer.DiscoveryAssembly(DatabaseAssembly assembly) {
            // What we return here is not proven valid. Types must be public. Properties
            // must be of a supported data type, etc.
            // TODO:

            var types = DiscoverDefinedTypes();

            var databaseTypes = assembly.DefineTypes(
                types.Select(t => Tuple.Create(t.FullName, t.BaseType?.FullName)).ToArray()
            );

            // After that, we have all types registered. Now we can
            // correctly discover and analyze all properties, knowing
            // the entire schema.

            foreach (var type in types) {
                var props = DiscoverDefinedProperties(type);
                foreach (var p in props) {
                    // We need the defining type to get it...
                }
            }
        }
    }
}
