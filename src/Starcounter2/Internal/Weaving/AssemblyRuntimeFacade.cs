
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

        public Type DatabaseAttributeType => typeof(DatabaseAttribute);

        public Type ProxyConstructorSignatureType => typeof(ProxyConstructorParameter);

        public Type InsertConstructorSignatureType => typeof(InsertConstructorParameter);

        public MethodInfo CreateMethod => crudProvider.GetCreateMethod();

        public IEnumerable<string> SupportedDataTypes => crudProvider.SupportedDataTypes;

        Type IAssemblyRuntimeFacade.DatabaseAttributeType => throw new NotImplementedException();

        Type IAssemblyRuntimeFacade.ProxyConstructorSignatureType => throw new NotImplementedException();

        Type IAssemblyRuntimeFacade.InsertConstructorSignatureType => throw new NotImplementedException();

        MethodInfo IAssemblyRuntimeFacade.CreateMethod => throw new NotImplementedException();

        IEnumerable<string> IAssemblyRuntimeFacade.SupportedDataTypes => throw new NotImplementedException();

        IEnumerable<RoutedInterfaceSpecification> IAssemblyRuntimeFacade.RoutedInterfaces => throw new NotImplementedException();

        Type IAssemblyRuntimeFacade.DbProxyStateInterfaceType => throw new NotImplementedException();

        public MethodInfo GetReadMethod(string type) {
            return crudProvider.GetReadMethod(type);
        }

        public MethodInfo GetWriteMethod(string type) {
            return crudProvider.GetUpdateMethod(type);
        }

        MethodInfo IAssemblyRuntimeFacade.GetReadMethod(string type) {
            throw new NotImplementedException();
        }

        MethodInfo IAssemblyRuntimeFacade.GetWriteMethod(string type) {
            throw new NotImplementedException();
        }
    }
}
