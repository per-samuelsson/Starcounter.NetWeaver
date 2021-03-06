﻿
using Starcounter.Weaver.Runtime;
using Starcounter.Weaver.Runtime.Abstractions;
using Starcounter.Weaver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace starweave.Weaver {

    public class StarcounterAssemblyRewriter : IAssemblyRewriter {
        readonly IWeaverHost host;
        readonly AnalysisResult analysis;
        readonly DatabaseTypeStateNames names;
        readonly CodeEmissionContext emitContext;
        readonly DatabaseTypeConstructorRewriter constructorRewriter;
        readonly IAssemblyRuntimeFacade runtimeFacade;
        readonly DatabaseTypeIDbProxyStateEmitter proxyInterfaceEmitter;
        readonly List<IEnumerable<RoutedInterfaceImplementation>> interfaceRouteStrategies;

        public StarcounterAssemblyRewriter(IWeaverHost weaverHost, AnalysisResult result, IAssemblyRuntimeFacade assemblyRuntimeFacade, DatabaseTypeStateNames stateNames) {
            host = weaverHost ?? throw new ArgumentNullException(nameof(weaverHost));
            analysis = result ?? throw new ArgumentNullException(nameof(result));
            runtimeFacade = assemblyRuntimeFacade ?? throw new ArgumentNullException(nameof(assemblyRuntimeFacade));
            names = stateNames ?? throw new ArgumentNullException(nameof(stateNames));
            emitContext = new CodeEmissionContext(result.SourceModule);

            proxyInterfaceEmitter = new DatabaseTypeIDbProxyStateEmitter(
                emitContext, 
                runtimeFacade.DbProxyStateInterfaceType
            );

            interfaceRouteStrategies = new List<IEnumerable<RoutedInterfaceImplementation>>(runtimeFacade.RoutedInterfaces.Count());
            foreach (var routedInterfaceSpecification in runtimeFacade.RoutedInterfaces) {
                var strategy = RoutedInterfaceImplementation.NewImplementationStrategy(
                    emitContext,
                    routedInterfaceSpecification.InterfaceType,
                    routedInterfaceSpecification.PassThroughType,
                    routedInterfaceSpecification.RoutingTarget
                    );
                interfaceRouteStrategies.Add(strategy);
            }
            
            constructorRewriter = new DatabaseTypeConstructorRewriter(
                host,
                emitContext,
                runtimeFacade.ProxyConstructorSignatureType,
                runtimeFacade.InsertConstructorSignatureType,
                runtimeFacade.CreateMethod
            );
        }
        
        void IAssemblyRewriter.RewriteType(DatabaseType type) {
            var module = analysis.SourceModule;

            var typeDef = type.GetTypeDefinition(module);
            var baseType = type.GetBaseType();
            var baseTypeDef = baseType != null ? typeDef.BaseType.Resolve() : null;

            var stateEmitter = new DatabaseTypeStateEmitter(emitContext, typeDef, names);
            stateEmitter.EmitCRUDHandles();
            if (baseType == null) {
                stateEmitter.EmitReferenceFields();
                proxyInterfaceEmitter.ImplementOn(typeDef, stateEmitter);
            }

            constructorRewriter.Rewrite(typeDef, baseTypeDef, stateEmitter);
            
            var propRewriter = new AutoImplementedPropertyRewriter(emitContext, stateEmitter);

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
                
                var readMethod = runtimeFacade.GetReadMethod(databaseProperty.DataType.Name);
                var writeMethod = runtimeFacade.GetWriteMethod(databaseProperty.DataType.Name);

                propRewriter.Rewrite(autoProperty, readMethod, writeMethod);
            }

            foreach (var routeStrategy in interfaceRouteStrategies) {
                foreach (var route in routeStrategy) {
                    route.ImplementOn(typeDef);
                }
            }
        }
    }
}