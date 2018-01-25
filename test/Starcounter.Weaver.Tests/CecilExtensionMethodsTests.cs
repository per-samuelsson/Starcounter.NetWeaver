
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

    class ClassDerivingObject : object { }
    class ClassDerivingOneBaseClass : ClassDerivingObject { }
    class ClassDerivingTwoBaseClasses : ClassDerivingOneBaseClass { }
    
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

        [Fact]
        public void TypesAreAssignableFromSelf() {
            var module = TestUtilities.GetModuleOfCurrentAssembly();

            var disposable = module.GetTypeReferences().Single(r => r.FullName == typeof(IDisposable).FullName).Resolve();
            Assert.True(disposable.IsAssignableFrom(disposable));

            var type = module.Types.Single(t => t.FullName == typeof(BaseClassWithField).FullName);
            Assert.True(type.IsAssignableFrom(type));

            type = module.Types.Single(t => t.FullName == typeof(SubClassWithNoField).FullName);
            Assert.True(type.IsAssignableFrom(type));

            type = module.Types.Single(t => t.FullName == typeof(SubClassThatInheritFieldAcrossAssembly).FullName);
            Assert.True(type.IsAssignableFrom(type));

            type = module.Types.Single(t => t.FullName == typeof(IInterfaceDerivingExternalInterface).FullName);
            Assert.True(type.IsAssignableFrom(type));
        }

        [Fact]
        public void ObjectIsAssignableFromAllTypes() {
            var module = TestUtilities.GetModuleOfCurrentAssembly();

            var @object = module.GetTypeReferences().Single(r => r.FullName == typeof(object).FullName).Resolve();
            Assert.True(@object.IsAssignableFrom(@object));

            var type = module.Types.Single(t => t.FullName == typeof(BaseClassWithField).FullName);
            Assert.True(@object.IsAssignableFrom(type));
            Assert.False(type.IsAssignableFrom(@object));

            type = module.Types.Single(t => t.FullName == typeof(SubClassWithNoField).FullName);
            Assert.True(@object.IsAssignableFrom(type));
            Assert.False(type.IsAssignableFrom(@object));

            type = module.Types.Single(t => t.FullName == typeof(SubClassThatInheritFieldAcrossAssembly).FullName);
            Assert.True(@object.IsAssignableFrom(type));
            Assert.False(type.IsAssignableFrom(@object));

            type = module.Types.Single(t => t.FullName == typeof(IInterfaceDerivingExternalInterface).FullName);
            Assert.True(@object.IsAssignableFrom(type));
            Assert.False(type.IsAssignableFrom(@object));
        }

        [Fact]
        public void BaseClassIsAssignableFromSubClass() {
            var module = TestUtilities.GetModuleOfCurrentAssembly();

            var baseType = module.Types.Single(t => t.FullName == typeof(BaseClassWithField).FullName);
            var type = module.Types.Single(t => t.FullName == typeof(SubClassWithNoField).FullName);

            Assert.True(baseType.IsAssignableFrom(type));
            Assert.False(type.IsAssignableFrom(baseType));
        }

        [Fact]
        public void GetBaseClassesReportErrorOnBadInput() {
            var module = TestUtilities.GetModuleOfCurrentAssembly();

            var interfaceType = module.Types.Single(t => t.FullName == typeof(IInterfaceDerivingExternalInterface).FullName);
            
            Assert.Throws<ArgumentNullException>(() => CecilExtensionMethods.GetBaseClasses(null));
            Assert.Throws<ArgumentException>(() => interfaceType.GetBaseClasses());
        }

        [Fact]
        public void GetBaseClassesReturnSingleWhenExtendingObject() {
            var module = TestUtilities.GetModuleOfCurrentAssembly();

            var type = module.Types.Single(t => t.FullName == typeof(ClassDerivingObject).FullName);
            
            Assert.Equal(1, type.GetBaseClasses().Count());
        }

        [Fact]
        public void GetBaseClassesReturnExpectedCounts() {
            var module = TestUtilities.GetModuleOfCurrentAssembly();

            var one = module.Types.Single(t => t.FullName == typeof(ClassDerivingOneBaseClass).FullName);
            var two = module.Types.Single(t => t.FullName == typeof(ClassDerivingTwoBaseClasses).FullName);

            Assert.Equal(2, one.GetBaseClasses().Count());
            Assert.Equal(3, two.GetBaseClasses().Count());
        }
    }
}