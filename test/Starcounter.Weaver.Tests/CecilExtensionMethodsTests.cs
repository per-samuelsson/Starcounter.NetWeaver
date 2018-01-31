
using Mono.Cecil;
using Mono.Cecil.Rocks;
using SharedTestUtilities;
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

    interface IEmptyInterface { }

    interface IInterfaceImplementingOne : IEmptyInterface { }

    interface IInterfaceImplementingTwoImplific : IInterfaceImplementingOne { }

    interface IInterfaceImplementingTwoExplicit : IEmptyInterface, IInterfaceImplementingOne { }

    interface IAnotherInterfaceImplementingOne : IEmptyInterface {}

    interface IInterfaceImplementingTwoImplementingOneCommon : IInterfaceImplementingOne, IAnotherInterfaceImplementingOne { }

    class ClassImplementingOne : IEmptyInterface { }

    class ClassImplementingTwoImplementingOneCommon : IInterfaceImplementingOne, IAnotherInterfaceImplementingOne { }

    class ClassWithEmptyStaticConstructor {
        static ClassWithEmptyStaticConstructor() {

        }
    }

    class ClassWithoutStaticConstructor {

    }

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
        public void CanGetReferenceToTypeGetTypeFromHandleMethod() {
            var module = TestUtilities.GetModuleOfCurrentAssembly();
            var getType = module.GetSingleReferencedMethod(typeof(Type), nameof(Type.GetTypeFromHandle));
            Assert.NotNull(getType);
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

        [Fact]
        public void GetAllInterfacesOnEmptyInterfaceReturnNone() {
            var module = TestUtilities.GetModuleOfCurrentAssembly();

            var empty = module.DefinitionOf(typeof(IEmptyInterface));
            Assert.Empty(empty.GetAllInterfaces());
        }

        [Fact]
        public void GetAllInterfacesOnInterfaceImplementingAnotherReturn1() {
            var module = TestUtilities.GetModuleOfCurrentAssembly();

            var one = module.DefinitionOf(typeof(IInterfaceImplementingOne));
            Assert.Equal(1, one.GetAllInterfaces().Count());
        }
        
        [Fact]
        public void GetAllInterfacesOnInterfaceImplementingTwoImplicitReturn2() {
            var module = TestUtilities.GetModuleOfCurrentAssembly();

            var two = module.DefinitionOf(typeof(IInterfaceImplementingTwoImplific));
            Assert.Equal(2, two.GetAllInterfaces().Count());
        }
        
        [Fact]
        public void GetAllInterfacesOnInterfaceImplementingTwoExplicitReturn2() {
            var module = TestUtilities.GetModuleOfCurrentAssembly();

            var two = module.DefinitionOf(typeof(IInterfaceImplementingTwoExplicit));
            Assert.Equal(2, two.GetAllInterfaces().Count());
        }
        
        [Fact]
        public void GetAllInterfacesOnInterfaceImplementingTwoWithCommonReturn3() {
            var module = TestUtilities.GetModuleOfCurrentAssembly();

            var three = module.DefinitionOf(typeof(IInterfaceImplementingTwoImplementingOneCommon));
            var result = three.GetAllInterfaces();

            Assert.Equal(3, result.Count());
            Assert.Contains(result, t => t.MetadataToken.ToInt32() == typeof(IEmptyInterface).MetadataToken);
            Assert.Contains(result, t => t.MetadataToken.ToInt32() == typeof(IInterfaceImplementingOne).MetadataToken);
            Assert.Contains(result, t => t.MetadataToken.ToInt32() == typeof(IAnotherInterfaceImplementingOne).MetadataToken);
        }

        [Fact]
        public void GetAllInterfacesOnClassImplementingOneInterfaceReturn1() {
            var module = TestUtilities.GetModuleOfCurrentAssembly();

            var one = module.DefinitionOf(typeof(ClassImplementingOne));
            var result = one.GetAllInterfaces();

            Assert.Equal(1, result.Count());
            Assert.Contains(result, t => t.MetadataToken.ToInt32() == typeof(IEmptyInterface).MetadataToken);
        }

        [Fact]
        public void GetAllInterfacesOnClassImplementingTwoWithCommonReturn3() {
            var module = TestUtilities.GetModuleOfCurrentAssembly();

            var three = module.DefinitionOf(typeof(ClassImplementingTwoImplementingOneCommon));
            var result = three.GetAllInterfaces();

            Assert.Equal(3, result.Count());
            Assert.Contains(result, t => t.MetadataToken.ToInt32() == typeof(IEmptyInterface).MetadataToken);
            Assert.Contains(result, t => t.MetadataToken.ToInt32() == typeof(IInterfaceImplementingOne).MetadataToken);
            Assert.Contains(result, t => t.MetadataToken.ToInt32() == typeof(IAnotherInterfaceImplementingOne).MetadataToken);
        }

        [Fact]
        public void CanFindDeclareStaticConstructor() {
            var module = TestUtilities.GetModuleOfCurrentAssembly();
            var type = module.DefinitionOf(typeof(ClassWithEmptyStaticConstructor));
            Assert.NotNull(type.GetStaticConstructor());
        }

        [Fact]
        public void CanEmitAndThenFindStaticConstructor() {
            var module = TestUtilities.GetModuleOfCurrentAssembly();
            var type = module.DefinitionOf(typeof(ClassWithoutStaticConstructor));

            Assert.Null(type.GetStaticConstructor());

            var cctor = CecilExtensionMethods.CreateStaticConstructorDefinition(module.TypeSystem);
            type.Methods.Add(cctor);

            Assert.NotNull(type.GetStaticConstructor());
        }
    }
}