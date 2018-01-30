
using Starcounter.Weaver.Runtime.Abstractions;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Starcounter2.Internal.Weaving {

    public sealed class AssemblyRuntimeFacade : IAssemblyRuntimeFacade {
        readonly DbCrudMethodProvider crudProvider;

        public AssemblyRuntimeFacade() {
            crudProvider = new DefaultDbCrudMethodProvider();
        }
        
        Type IAssemblyRuntimeFacade.DatabaseAttributeType => typeof(DatabaseAttribute);

        Type IAssemblyRuntimeFacade.ProxyConstructorSignatureType => typeof(ProxyConstructorParameter);

        Type IAssemblyRuntimeFacade.InsertConstructorSignatureType => typeof(InsertConstructorParameter);

        MethodInfo IAssemblyRuntimeFacade.CreateMethod => crudProvider.GetCreateMethod();

        IEnumerable<string> IAssemblyRuntimeFacade.SupportedDataTypes => crudProvider.SupportedDataTypes;

        IEnumerable<RoutedInterfaceSpecification> IAssemblyRuntimeFacade.RoutedInterfaces => throw new NotImplementedException();

        Type IAssemblyRuntimeFacade.DbProxyStateInterfaceType => throw new NotImplementedException();
        
        MethodInfo IAssemblyRuntimeFacade.GetReadMethod(string type) {
            return crudProvider.GetReadMethod(type);
        }

        MethodInfo IAssemblyRuntimeFacade.GetWriteMethod(string type) {
            return crudProvider.GetUpdateMethod(type);
        }
    }
}
