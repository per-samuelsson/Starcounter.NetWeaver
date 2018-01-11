
using Starcounter.Weaver.Runtime.Abstractions;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace Starcounter2.Internal {

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

        public MethodInfo GetReadMethod(string type) {
            return crudProvider.GetReadMethod(type);
        }

        public MethodInfo GetWriteMethod(string type) {
            return crudProvider.GetUpdateMethod(type);
        }
    }
}
