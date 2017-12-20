
using Mono.Cecil;
using Starcounter.Weaver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace starweave.Weaver {

    public class StarcounterAssemblyAnalyzer : IAssemblyAnalyzer {
        readonly IWeaverHost host;
        readonly ModuleDefinition module;
        readonly string target;
        readonly TargetRuntimeFacadeProvider runtimeProvider;
        readonly IEnumerable<string> dataTypes;
        
        public StarcounterAssemblyAnalyzer(IWeaverHost weaverHost, ModuleDefinition moduleDefinition, TargetRuntimeFacadeProvider targetRuntimeProvider, IEnumerable<string> supportedDataTypes) {
            host = weaverHost ?? throw new ArgumentNullException(nameof(weaverHost));
            module = moduleDefinition ?? throw new ArgumentNullException(nameof(moduleDefinition));
            runtimeProvider = targetRuntimeProvider ?? throw new ArgumentNullException(nameof(targetRuntimeProvider));
            dataTypes = supportedDataTypes ?? throw new ArgumentNullException(nameof(supportedDataTypes));
        }
        
        bool IAssemblyAnalyzer.IsTargetReference(ModuleDefinition module) {
            return runtimeProvider.IsTargetRuntimeReference(module);
        }

        void IAssemblyAnalyzer.DiscoveryAssembly(AnalysisResult analysisResult) {
            var runtime = runtimeProvider.ProvideRuntimeFacade(analysisResult.TargetModule);

            // Supported data types must come from the facade.
            // TODO:

            var assembly = analysisResult.AnalyzedAssembly;

            foreach (var dataType in dataTypes) {
                assembly.DefiningSchema.DefineDataType(dataType);
            }
            
            var types = DiscoverDefinedDatabaseTypes(runtime.DatabaseAttributeType);

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

        IEnumerable<TypeDefinition> DiscoverDefinedDatabaseTypes(Type databaseAttributeType) {
            // Custom attribute type must come from the target
            return module.Types.Where(t => t.HasCustomAttribute(databaseAttributeType));
        }

        IEnumerable<PropertyDefinition> DiscoverDefinedDatabaseProperties(TypeDefinition type) {
            return type.Properties.Where(p => p.IsAutoImplemented());
        }
    }
}
