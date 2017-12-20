
using Mono.Cecil;
using Starcounter.Hosting;
using Starcounter.Weaver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace starweave.Weaver {

    public class StarcounterAssemblyAnalyzer : IAssemblyAnalyzer {
        readonly IWeaverHost host;
        readonly ModuleDefinition module;
        readonly TargetRuntimeFacadeProvider runtimeProvider;

        public IAssemblyRuntimeFacade RuntimeFacade { get; private set; }
        
        public StarcounterAssemblyAnalyzer(IWeaverHost weaverHost, ModuleDefinition moduleDefinition, TargetRuntimeFacadeProvider targetRuntimeProvider) {
            host = weaverHost ?? throw new ArgumentNullException(nameof(weaverHost));
            module = moduleDefinition ?? throw new ArgumentNullException(nameof(moduleDefinition));
            runtimeProvider = targetRuntimeProvider ?? throw new ArgumentNullException(nameof(targetRuntimeProvider));
        }
        
        bool IAssemblyAnalyzer.IsTargetReference(ModuleDefinition module) {
            return runtimeProvider.IsTargetRuntimeReference(module);
        }

        void IAssemblyAnalyzer.DiscoveryAssembly(AnalysisResult analysisResult) {
            var runtime = RuntimeFacade = runtimeProvider.ProvideRuntimeFacade(analysisResult.TargetModule);

            // Also: bind future weaved assembly to the given runtime. It's kind of
            // part of the contract too. And it's nice to see that in weaved result:
            // full identity of the assembly, what it has provided, etc.
            // TODO:
            
            var assembly = analysisResult.AnalyzedAssembly;

            foreach (var dataType in runtime.SupportedDataTypes) {
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
            return module.Types.Where(t => t.HasCustomAttribute(databaseAttributeType));
        }

        IEnumerable<PropertyDefinition> DiscoverDefinedDatabaseProperties(TypeDefinition type) {
            return type.Properties.Where(p => p.IsAutoImplemented());
        }
    }
}
