
using Starcounter.Weaver.Tests.ExternalCode;
using System;
using System.Linq;
using Xunit;

namespace Starcounter.Weaver.Tests {

    class BaseClassWithField {
        protected int Field1;
    }

    class SubClassWithNoField : BaseClassWithField {

    }

    class SubClassThatInheritFieldAcrossAssembly : ClassDerivingExplicitStateReferenceFieldsBase {

    }

    interface IInterfaceDerivingExternalInterface : IDisposable {
    }

    class ClassImplementingIInterfaceDerivingExternalInterface : IInterfaceDerivingExternalInterface {
        void IDisposable.Dispose() {
            throw new NotImplementedException();
        }
    }
    class ClassDerivingClassImplementingIInterfaceDerivingExternalInterface : ClassImplementingIInterfaceDerivingExternalInterface { }

    public class CecilExtensionMethodsTests {

        [Fact]
        public void CanFindFieldByNameInBaseClassSameAssembly() {
            var module = TestUtilities.GetModuleOfCurrentAssembly();
            var type = module.Types.Single(t => t.FullName == typeof(SubClassWithNoField).FullName);
            var f = type.GetFieldRecursive("Field1");
            Assert.NotNull(f);
        }

        [Fact]
        public void CanFindFieldByNameInBaseClassAcrossAssemblies() {
            var module = TestUtilities.GetModuleOfCurrentAssembly();
            var type = module.Types.Single(t => t.FullName == typeof(SubClassThatInheritFieldAcrossAssembly).FullName);
            var f = type.GetFieldRecursive("dbId");
            Assert.NotNull(f);
        }

        [Fact]
        public void CanResolveObjectConstructorMethodReference() {
            var module = TestUtilities.GetModuleOfCurrentAssembly();
            var oc = module.GetObjectConstructorReference();
            Assert.NotNull(oc);
        }

        [Fact]
        public void ImplementInterfaceDetectInheritedScenarios() {
            var module = TestUtilities.GetModuleOfCurrentAssembly();
            
            var disposable = module.GetTypeReferences().Single(r => r.FullName == typeof(IDisposable).FullName);
            Assert.NotNull(disposable);

            var type = module.Types.Single(t => t.FullName == typeof(ClassImplementingIInterfaceDerivingExternalInterface).FullName);
            Assert.NotNull(type);
            Assert.True(type.ImplementInterface(disposable));

            type = module.Types.Single(t => t.FullName == typeof(ClassDerivingClassImplementingIInterfaceDerivingExternalInterface).FullName);
            Assert.NotNull(type);
            Assert.True(type.ImplementInterface(disposable));
        }
    }
}