
using Starcounter.Hosting;
using starweave.Weaver;
using System;
using System.Collections.Generic;
using Xunit;
using System.Reflection;

namespace starweave.Tests {

    public class ThisTypeDontImplementAnyInterface { }

    public abstract class AbstractImplementationOfRuntimeFacade : IAssemblyRuntimeFacade {

        public abstract Type DatabaseAttributeType { get; }
        public abstract Type ProxyConstructorSignatureType { get; }
        public abstract Type InsertConstructorSignatureType { get; }
        public abstract MethodInfo CreateMethod { get; }
        public abstract IEnumerable<string> SupportedDataTypes { get; }

        public abstract MethodInfo GetReadMethod(string type);
        public abstract MethodInfo GetWriteMethod(string type);
    }

    public class ImplementationOfRuntimeFacadeWithNoDefaultConstructor : IAssemblyRuntimeFacade {
        public ImplementationOfRuntimeFacadeWithNoDefaultConstructor(int dummy) { }

        Type IAssemblyRuntimeFacade.DatabaseAttributeType => throw new NotImplementedException();
        Type IAssemblyRuntimeFacade.ProxyConstructorSignatureType => throw new NotImplementedException();
        Type IAssemblyRuntimeFacade.InsertConstructorSignatureType => throw new NotImplementedException();
        MethodInfo IAssemblyRuntimeFacade.CreateMethod => throw new NotImplementedException();
        IEnumerable<string> IAssemblyRuntimeFacade.SupportedDataTypes => throw new NotImplementedException();
        MethodInfo IAssemblyRuntimeFacade.GetReadMethod(string type) {
            throw new NotImplementedException();
        }
        MethodInfo IAssemblyRuntimeFacade.GetWriteMethod(string type) {
            throw new NotImplementedException();
        }
    }

    public class AssemblyLoadTargetRuntimeProviderTests {

        [Fact]
        void MethodIsValidRuntimeFacadeProduceExpectedResultsOnFailingTypes() {
            var types = new[] {
                typeof(IAssemblyRuntimeFacade),
                typeof(ThisTypeDontImplementAnyInterface),
                typeof(AbstractImplementationOfRuntimeFacade),
                typeof(ImplementationOfRuntimeFacadeWithNoDefaultConstructor)
            };
            foreach (var type in types) {
                var result = AssemblyLoadTargetRuntimeProvider.IsValidRuntimeFacade(type);
                Assert.False(result);
            }
        }
    }
}
