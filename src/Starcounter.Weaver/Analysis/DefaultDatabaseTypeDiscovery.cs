
using Mono.Cecil;
using Starcounter.Hosting.Schema;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Starcounter.Weaver.Analysis {

    public class DefaultDatabaseTypeDiscovery : IDatabaseTypeDiscovery {

        // What we return here is not proven valid. Types must be public. Properties
        // must be of a supported data type, etc.
        // TODO:

        protected virtual IEnumerable<TypeDefinition> DiscoverDefinedTypes(ModuleDefinition module) {
            return module.Types.Where(t => t.HasCustomAttribute(typeof(Starcounter2.DatabaseAttribute)));
        }

        protected virtual IEnumerable<PropertyDefinition> DiscoverDefinedProperties(TypeDefinition type) {
            return type.Properties.Where(p => p.IsAutoImplemented());
        }

        DatabaseSchema IDatabaseTypeDiscovery.DiscoverAssembly(ModuleDefinition module, DatabaseAssembly assembly) {
            var types = DiscoverDefinedTypes(module);
            var databaseTypes = assembly.DefineTypes(
                types.Select(t => Tuple.Create(t.FullName, t.BaseType.FullName)).ToArray()
            );

            foreach (var type in types) {
                var props = DiscoverDefinedProperties(type);
                foreach (var p in props) {
                    // We need the defining type to get it...
                }
            }

            // After that, we have all types registered. Now we can
            // correctly discover and analyze all properties, knowing
            // the entire schema.

            return assembly.DefiningSchema;
        }
    }
}