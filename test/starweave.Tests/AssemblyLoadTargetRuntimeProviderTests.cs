
using Starcounter.Hosting;
using starweave.Weaver;
using System;
using System.Collections.Generic;
using Xunit;
using System.Reflection;
using Starcounter.Weaver;
using Mono.Cecil;
using starweave.Weaver.Tests;

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

    class AssemblyLoadTargetRuntimeProviderUsingGivenAssembly : AssemblyLoadTargetRuntimeProvider {
        protected readonly Assembly assembly;

        public AssemblyLoadTargetRuntimeProviderUsingGivenAssembly(
            IWeaverHost weaverHost,
            string targetRuntimeAssemblyIdentity,
            Assembly givenAssembly) : base(weaverHost, targetRuntimeAssemblyIdentity) {

            assembly = givenAssembly;
        }

        protected override Assembly LoadRuntimeAssembly(ModuleDefinition targetReference) {
            return assembly;
        }
    }

    class AssemblyLoadTargetRuntimeProviderWithNoMatchingType : AssemblyLoadTargetRuntimeProviderUsingGivenAssembly {
        
        public AssemblyLoadTargetRuntimeProviderWithNoMatchingType(
            IWeaverHost weaverHost, 
            string targetRuntimeAssemblyIdentity,
            Assembly givenAssembly) : base(weaverHost, targetRuntimeAssemblyIdentity, givenAssembly) {
        }
        
        protected override Type FindRuntimeType(Assembly assembly) {
            return null;
        }
    }

    public class AssemblyLoadTargetRuntimeProviderTests {

        [Fact]
        public void MethodIsValidRuntimeFacadeProduceExpectedResultsOnFailingTypes() {
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

        [Fact]
        public void AssemblyWithNoRuntimeTypeProduceMeaningfulError() {
            var thisModule = TestUtilities.GetModuleOfCurrentAssembly();
            var thisAssembly = Assembly.GetExecutingAssembly();

            var provider = new AssemblyLoadTargetRuntimeProviderWithNoMatchingType(
                new DefaultWeaverHost(TestUtilities.QuietDiagnostics), 
                "dummy", 
                thisAssembly
            );

            var e = Assert.Throws<InvalidOperationException>(() => provider.ProvideRuntimeFacade(thisModule));
            Assert.Contains(thisAssembly.GetName().Name, e.Message);
        }
    }
}
