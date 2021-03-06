﻿using Mono.Cecil;
using Starcounter.Weaver.Runtime;

namespace Starcounter.Weaver.Analysis {

    public abstract class PreAnalysis {
        readonly ModuleReferenceDiscovery refDiscovery;
        readonly SchemaSerializationContext serializationContext;
        readonly WeaverDiagnostics diag;
        
        protected PreAnalysis(
            ModuleReferenceDiscovery referenceDiscovery,
            SchemaSerializationContext schemaSerializationContext,
            WeaverDiagnostics diagnostics) {
            Guard.NotNull(referenceDiscovery, nameof(referenceDiscovery));
            Guard.NotNull(schemaSerializationContext, nameof(schemaSerializationContext));
            Guard.NotNull(diagnostics, nameof(diagnostics));

            refDiscovery = referenceDiscovery;
            serializationContext = schemaSerializationContext;
            diag = diagnostics;
        }

        public void Execute(ModuleDefinition module, IAssemblyAnalyzer analyzer, out ModuleDefinition target, out DatabaseSchema externalSchema) {
            target = null;
            externalSchema = null;
            
            var referenceFinder = SingleModuleReferenceFinder.Run(module, refDiscovery, (m) => {
                return analyzer.IsTargetReference(m);
            });

            if (referenceFinder.Result == null) {
                return;
            }

            target = referenceFinder.Result;
            externalSchema = new DatabaseSchema();

            foreach (var referenceDiscovered in referenceFinder.ModulesConsidered) {
                var moduleSchema = DiscoverSchema(referenceDiscovered, serializationContext);
                if (moduleSchema != null) {
                    externalSchema = externalSchema.MergeWith(moduleSchema);
                }
            }
        }
        
        protected abstract DatabaseSchema DiscoverSchema(ModuleDefinition candidate, SchemaSerializationContext serializationContext);
    }
}
