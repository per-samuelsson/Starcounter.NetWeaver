
using SharedTestUtilities;
using System;
using System.Linq;
using Xunit;
using Starcounter.Weaver.Tests.ExternalCode;

namespace Starcounter.Weaver.Tests {

    interface IInterfaceWithoutMembers { }

    interface IInterfaceWithMethods {
        void Method1();

        int Method2();

        string Method3(object o);

        bool Method4(int i, short s, string s2, bool b, object o, DateTime dt, IInterfaceWithMethods x, char c, TimeSpan ts, int i2, int i3, long l1);
    }

    interface IInterfaceWithProperties {
        int ReadOnlyProperty { get; }
        int ReadWriteProperty { get; set; }
    }

    interface IPassThroughType {}

    static class RoutingTargetType {

        public static void Method1(IPassThroughType t) {

        }

        public static int Method2(IPassThroughType t) {
            return 42;
        }

        public static string Method3(IPassThroughType t, object o) {
            return "42";
        }

        public static bool Method4(IPassThroughType t, int i, short s, string s2, bool b, object o, DateTime dt, IInterfaceWithMethods x, char c, TimeSpan ts, int i2, int i3, long l1) {
            return true;
        }

        public static int Get_ReadOnlyProperty(IPassThroughType t) {
            return 42;
        }

        public static int Get_ReadWriteProperty(IPassThroughType t) {
            return 42;
        }

        public static void Set_ReadWriteProperty(IPassThroughType t, int i) {
        }
    }

    class EmptyType : IPassThroughType {
    }
    
    class EmptyType_Solution : IPassThroughType, IInterfaceWithMethods {

        void IInterfaceWithMethods.Method1() {
            RoutingTargetType.Method1(this);
        }

        int IInterfaceWithMethods.Method2() {
            return RoutingTargetType.Method2(this);
        }

        string IInterfaceWithMethods.Method3(object o) {
            return RoutingTargetType.Method3(this, o);
        }

        bool IInterfaceWithMethods.Method4(int i, short s, string s2, bool b, object o, DateTime dt, IInterfaceWithMethods x, char c, TimeSpan ts, int i2, int i3, long l1) {
            return RoutingTargetType.Method4(this, i, s, s2, b, o, dt, x, c, ts, i2, i3, l1);
        }
    }

    class TypeWithInterfaceProperties: IPassThroughType {

    }

    class TypeWithInterfaceProperties_Solution : IPassThroughType, IInterfaceWithProperties {

        int IInterfaceWithProperties.ReadOnlyProperty => RoutingTargetType.Get_ReadOnlyProperty(this);

        int IInterfaceWithProperties.ReadWriteProperty {
            get => RoutingTargetType.Get_ReadWriteProperty(this);
            set => RoutingTargetType.Set_ReadWriteProperty(this, value);
        }
    }
    
    class WrongPassThroughType { }

    interface IRootBaseInterface { }

    interface IBaseInterface : IRootBaseInterface { }

    interface IBaseInterface2 {
        void Base2();
    }

    interface IExtendedInterfaceSimple : IRootBaseInterface { }

    interface IExtendedInterface : IBaseInterface, IBaseInterface2 {
        void Extended();
    }

    interface IExtendExtendedDisposableClonableAndCustomFormatter : IExtendedInterface, IDisposable, ICloneable, ICustomFormatter { }

    class RoutingTypeForExtendedInterface {
        
        public static void Base2(IPassThroughType cargo) {
            throw new NotImplementedException();
        }

        public static void Extended(IPassThroughType cargo) {
            throw new NotImplementedException();
        }

        public static void Dispose(IPassThroughType cargo) {
            throw new NotImplementedException();
        }

        public static object Clone(IPassThroughType cargo) {
            throw new NotImplementedException();
        }

        public static string Format(IPassThroughType cargo, string format, object arg, IFormatProvider formatProvider) {
            throw new NotImplementedException();
        }
    }

    class ClassThatWillReceiveRoutingInterfaceFromExternalAssembly : IDbProxyInExternalAssembly {
        ulong IDbProxyInExternalAssembly.GetDbId() {
            throw new NotImplementedException();
        }

        ulong IDbProxyInExternalAssembly.GetDbRef() {
            throw new NotImplementedException();
        }

        void IDbProxyInExternalAssembly.SetDbId(ulong id) {
            throw new NotImplementedException();
        }

        void IDbProxyInExternalAssembly.SetDbRef(ulong @ref) {
            throw new NotImplementedException();
        }
    }

    public class RoutedInterfaceImplementationTests {

        [Fact]
        public void BadInputGiveMeaningfulErrors() {
            var module = TestUtilities.GetModuleOfCurrentAssembly();

            var emitContext = new CodeEmissionContext(module);
            var interfaceDefinition = module.Types.Single(t => t.FullName == typeof(IInterfaceWithMethods).FullName);
            var passThrough = module.Types.Single(t => t.FullName == typeof(IPassThroughType).FullName);
            var routingTargetType = module.Types.Single(t => t.FullName == typeof(RoutingTargetType).FullName);

            Assert.Throws<ArgumentNullException>(() => new RoutedInterfaceImplementation(
                null, interfaceDefinition, passThrough, routingTargetType));

            Assert.Throws<ArgumentNullException>(() => new RoutedInterfaceImplementation(
                emitContext, null, passThrough, routingTargetType));

            Assert.Throws<ArgumentNullException>(() => new RoutedInterfaceImplementation(
                emitContext, interfaceDefinition, null, routingTargetType));

            Assert.Throws<ArgumentNullException>(() => new RoutedInterfaceImplementation(
                emitContext, interfaceDefinition, passThrough, null));

            var ae = Assert.Throws<ArgumentException>(() => new RoutedInterfaceImplementation(
                emitContext, routingTargetType, passThrough, routingTargetType));
            Assert.NotEmpty(ae.ParamName);

            ae = Assert.Throws<ArgumentException>(() => new RoutedInterfaceImplementation(
                emitContext, interfaceDefinition, routingTargetType, routingTargetType));
            Assert.NotEmpty(ae.ParamName);

            var implementation = new RoutedInterfaceImplementation(emitContext, interfaceDefinition, passThrough, routingTargetType);

            var typeNotImplementingPassThrough = module.Types.Single(t => t.FullName == typeof(WrongPassThroughType).FullName);
            Assert.Throws<ArgumentException>(() => { implementation.ImplementOn(typeNotImplementingPassThrough); });
        }

        [Fact]
        public void MethodRoutesProduceInterfaceImplementation() {

            using (var m = TestUtilities.GetModuleOfCurrentAssemblyForRewriting()) {
                var module = m.Module;

                var interfaceDefinition = module.Types.Single(t => t.FullName == typeof(IInterfaceWithMethods).FullName);
                var passThrough = module.Types.Single(t => t.FullName == typeof(IPassThroughType).FullName);
                var routingTargetType = module.Types.Single(t => t.FullName == typeof(RoutingTargetType).FullName);

                var implementation = new RoutedInterfaceImplementation(new CodeEmissionContext(module), interfaceDefinition, passThrough, routingTargetType);

                var target = module.Types.Single(t => t.FullName == typeof(EmptyType).FullName);
                var initialTargetMethodCount = 1; //compiler-emitted .ctor
                Assert.False(target.ImplementInterface(interfaceDefinition));
                Assert.Equal(initialTargetMethodCount, target.Methods.Count);  

                implementation.ImplementOn(target);
                Assert.True(target.ImplementInterface(interfaceDefinition));
                Assert.Equal(interfaceDefinition.Methods.Count + initialTargetMethodCount, target.Methods.Count);

                foreach (var method in target.Methods) {
                    if (!method.HasOverrides) continue;
                    Assert.NotNull(MethodCallFinder.FindSingleCallToAnyTarget(method, routingTargetType.Methods));
                }
            }
        }

        [Fact]
        public void PropertyRoutesProduceInterfaceImplementation() {

            using (var m = TestUtilities.GetModuleOfCurrentAssemblyForRewriting()) {
                var module = m.Module;

                var interfaceDefinition = module.Types.Single(t => t.FullName == typeof(IInterfaceWithProperties).FullName);
                var passThrough = module.Types.Single(t => t.FullName == typeof(IPassThroughType).FullName);
                var routingTargetType = module.Types.Single(t => t.FullName == typeof(RoutingTargetType).FullName);

                var implementation = new RoutedInterfaceImplementation(new CodeEmissionContext(module), interfaceDefinition, passThrough, routingTargetType);

                var target = module.Types.Single(t => t.FullName == typeof(TypeWithInterfaceProperties).FullName);
                var initialTargetMethodCount = 1; //compiler-emitted .ctor
                Assert.False(target.ImplementInterface(interfaceDefinition));
                Assert.Equal(initialTargetMethodCount, target.Methods.Count);
                Assert.False(target.HasProperties);

                implementation.ImplementOn(target);
                Assert.True(target.ImplementInterface(interfaceDefinition));
                Assert.Equal(interfaceDefinition.Properties.Count, target.Properties.Count);
                Assert.Equal(interfaceDefinition.Methods.Count + initialTargetMethodCount, target.Methods.Count);

                foreach (var method in target.Methods) {
                    if (!method.HasOverrides) continue;
                    Assert.NotNull(MethodCallFinder.FindSingleCallToAnyTarget(method, routingTargetType.Methods));
                }
            }
        }

        [Fact]
        public void ShouldRaiseMeaningfulErrorWhenInterfaceIsImplementedTwice() {
            
            using (var m = TestUtilities.GetModuleOfCurrentAssemblyForRewriting()) {
                var module = m.Module;

                var emptyInterface = module.Types.Single(t => t.FullName == typeof(IInterfaceWithoutMembers).FullName);
                var passThrough = module.Types.Single(t => t.FullName == typeof(IPassThroughType).FullName);
                var routingTargetType = module.Types.Single(t => t.FullName == typeof(RoutingTypeForExtendedInterface).FullName);

                var implementation = new RoutedInterfaceImplementation(new CodeEmissionContext(module), emptyInterface, passThrough, routingTargetType);
                var target = module.Types.Single(t => t.FullName == typeof(EmptyType).FullName);

                implementation.ImplementOn(target);
                var e = Assert.Throws<ArgumentException>(() => implementation.ImplementOn(target));
                Assert.Contains(target.Name, e.Message);
                Assert.Contains(emptyInterface.Name, e.Message);
            }
        }

        [Fact]
        public void ShouldReportErrorForInterfaceWithSimpleBaseInterfacesNotImplemented() {

            using (var m = TestUtilities.GetModuleOfCurrentAssemblyForRewriting()) {
                var module = m.Module;

                var extendedInterface = module.Types.Single(t => t.FullName == typeof(IExtendedInterfaceSimple).FullName);
                var passThrough = module.Types.Single(t => t.FullName == typeof(IPassThroughType).FullName);
                var routingTargetType = module.Types.Single(t => t.FullName == typeof(RoutingTypeForExtendedInterface).FullName);
                
                var implementation = new RoutedInterfaceImplementation(new CodeEmissionContext(module), extendedInterface, passThrough, routingTargetType);
                var target = module.Types.Single(t => t.FullName == typeof(EmptyType).FullName);

                var e = Assert.Throws<ArgumentException>(() => implementation.ImplementOn(target));
                Assert.Contains(target.Name, e.Message);
                Assert.Contains(typeof(IRootBaseInterface).Name, e.Message);
            }
        }

        [Fact]
        public void RoutingTargetMethodsCanBeQualified() {
            var module = TestUtilities.GetModuleOfCurrentAssembly();
            
            var extendedInterface = module.DefinitionOf(typeof(IExtendExtendedDisposableClonableAndCustomFormatter));
            var formatterInterface = extendedInterface.GetAllInterfaces().Single(i => i.MetadataToken.ToInt32() == typeof(ICustomFormatter).MetadataToken);

            var format = formatterInterface.Methods.Single(m => m.Name == nameof(ICustomFormatter.Format));

            var targetType = module.DefinitionOf(typeof(RoutingTypeForExtendedInterface));

            var pt = module.DefinitionOf(typeof(IPassThroughType));
            var formatRoutingTarget = targetType.Methods.FirstOrDefault(
                method => RoutedInterfaceImplementation.IsQualifiedRoutingTarget(
                    method, 
                    format,
                    pt));
            Assert.NotNull(formatRoutingTarget);
        }

        [Fact]
        public void NewImplementationStrategySupportInterfaceNotImplementingAnyOther() {

            using (var m = TestUtilities.GetModuleOfCurrentAssemblyForRewriting()) {
                var module = m.Module;

                var root = module.DefinitionOf(typeof(IRootBaseInterface));
                var @base = module.DefinitionOf(typeof(IBaseInterface));
                var base2 = module.DefinitionOf(typeof(IBaseInterface2));
                var extended = module.DefinitionOf(typeof(IExtendedInterface));
                var extendExtended = module.DefinitionOf(typeof(IExtendExtendedDisposableClonableAndCustomFormatter));

                var passThrough = module.DefinitionOf(typeof(IPassThroughType));
                var routingTargetType = module.DefinitionOf(typeof(RoutingTypeForExtendedInterface));

                var context = new CodeEmissionContext(module);
                var target = module.DefinitionOf(typeof(EmptyType));

                var test = RoutedInterfaceImplementation.NewImplementationStrategy(
                    context,
                    root,
                    passThrough,
                    routingTargetType
                );

                Assert.Equal(1, test.Count());
                Assert.Equal(root.FullName, test.First().InterfaceType.FullName);
            }
        }

        [Fact]
        public void NewImplementationStrategySupportInterfaceWithSingleImplementation() {
            // interface IRootBaseInterface { }
            // interface IBaseInterface : IRootBaseInterface { }
            
            using (var m = TestUtilities.GetModuleOfCurrentAssemblyForRewriting()) {
                var module = m.Module;

                var root = module.DefinitionOf(typeof(IRootBaseInterface));
                var @base = module.DefinitionOf(typeof(IBaseInterface));

                var passThrough = module.DefinitionOf(typeof(IPassThroughType));
                var routingTargetType = module.DefinitionOf(typeof(RoutingTypeForExtendedInterface));

                var context = new CodeEmissionContext(module);
                var target = module.DefinitionOf(typeof(EmptyType));
                
                var test = RoutedInterfaceImplementation.NewImplementationStrategy(
                    context,
                    @base,
                    passThrough,
                    routingTargetType
                );

                Assert.Equal(2, test.Count());
                Assert.Equal(root.FullName, test.First().InterfaceType.FullName);
                Assert.Equal(@base.FullName, test.Last().InterfaceType.FullName);
            }
        }

        [Fact]
        public void NewImplementationStrategySupportInterfaceWithMultipleImplementations() {

            // interface IRootBaseInterface { }
            // interface IBaseInterface : IRootBaseInterface { }
            // interface IBaseInterface2 { 
            //   void Base2();
            // }
            // interface IExtendedInterface : IBaseInterface, IBaseInterface2 {
            //   void Extended();
            // }
            
            using (var m = TestUtilities.GetModuleOfCurrentAssemblyForRewriting()) {
                var module = m.Module;

                var root = module.DefinitionOf(typeof(IRootBaseInterface));
                var extended = module.DefinitionOf(typeof(IExtendedInterface));
                
                var passThrough = module.DefinitionOf(typeof(IPassThroughType));
                var routingTargetType = module.DefinitionOf(typeof(RoutingTypeForExtendedInterface));

                var context = new CodeEmissionContext(module);
                var target = module.DefinitionOf(typeof(EmptyType));
                
                var test = RoutedInterfaceImplementation.NewImplementationStrategy(
                    context,
                    extended,
                    passThrough,
                    routingTargetType
                );

                Assert.Equal(4, test.Count());
                Assert.Equal(root.FullName, test.First().InterfaceType.FullName);
                Assert.Equal(extended.FullName, test.Last().InterfaceType.FullName);
            }
        }


        [Fact]
        public void NewImplementationStrategySupportInterfaceWithMultipleImplementationsFromDifferentAssemblies() {
            // interface IRootBaseInterface { }
            // interface IBaseInterface : IRootBaseInterface { }
            // interface IBaseInterface2 { 
            //   void Base2();
            // }
            // interface IExtendedInterface : IBaseInterface, IBaseInterface2 {
            //   void Extended();
            // }
            // interface IExtendExtendedDisposableClonableAndAsyncResult : IExtendedInterface, IDisposable, ICloneable, ICustomFormatter { }

            using (var m = TestUtilities.GetModuleOfCurrentAssemblyForRewriting()) {
                var module = m.Module;

                var root = module.DefinitionOf(typeof(IRootBaseInterface));
                var @base = module.DefinitionOf(typeof(IBaseInterface));
                var base2 = module.DefinitionOf(typeof(IBaseInterface2));
                var extended = module.DefinitionOf(typeof(IExtendedInterface));
                var extendExtended = module.DefinitionOf(typeof(IExtendExtendedDisposableClonableAndCustomFormatter));

                var passThrough = module.DefinitionOf(typeof(IPassThroughType));
                var routingTargetType = module.DefinitionOf(typeof(RoutingTypeForExtendedInterface));

                var context = new CodeEmissionContext(module);
                var target = module.DefinitionOf(typeof(EmptyType));

                var test = RoutedInterfaceImplementation.NewImplementationStrategy(
                    context,
                    extendExtended,
                    passThrough,
                    routingTargetType
                );

                Assert.True(test.Count() >= 8);
                Assert.Equal(extendExtended.FullName, test.Last().InterfaceType.FullName);

                var list = test.ToList();
                Assert.Equal(extendExtended.FullName, list.Last().InterfaceType.FullName);
                
                var rootIndex = list.FindIndex(r => r.InterfaceType.FullName == root.FullName);
                var baseIndex = list.FindIndex(r => r.InterfaceType.FullName == @base.FullName);
                var base2Index = list.FindIndex(r => r.InterfaceType.FullName == base2.FullName);
                var clonableIndex = list.FindIndex(r => r.InterfaceType.FullName == typeof(ICloneable).FullName);
                var extendExtendedIndex = list.FindIndex(r => r.InterfaceType.FullName == extendExtended.FullName);

                Assert.InRange(baseIndex, rootIndex, extendExtendedIndex);
                Assert.InRange(base2Index, rootIndex, extendExtendedIndex);
                Assert.True(clonableIndex < extendExtendedIndex);
            }
        }

        [Fact]
        public void ExtendedInterfacesCanOnlyBeImplementedAfterDependenciesAre() {
            // interface IRootBaseInterface { }
            // interface IBaseInterface : IRootBaseInterface { }
            // interface IBaseInterface2 { 
            //   void Base2();
            // }
            // interface IExtendedInterface : IBaseInterface, IBaseInterface2 {
            //   void Extended();
            // }

            using (var m = TestUtilities.GetModuleOfCurrentAssemblyForRewriting()) {
                var module = m.Module;

                var root = module.DefinitionOf(typeof(IRootBaseInterface));
                var @base = module.DefinitionOf(typeof(IBaseInterface));
                var base2 = module.DefinitionOf(typeof(IBaseInterface2));
                var extended = module.DefinitionOf(typeof(IExtendedInterface));

                var passThrough = module.DefinitionOf(typeof(IPassThroughType));
                var routingTargetType = module.DefinitionOf(typeof(RoutingTypeForExtendedInterface));
                
                var context = new CodeEmissionContext(module);
                var target = module.DefinitionOf(typeof(EmptyType));
                
                var extendedImpl = new RoutedInterfaceImplementation(context, extended, passThrough, routingTargetType);
                
                var e = Assert.Throws<ArgumentException>(() => extendedImpl.ImplementOn(target));
                Assert.Contains(target.Name, e.Message);

                var strategy = RoutedInterfaceImplementation.NewImplementationStrategy(
                    context, 
                    extended, 
                    passThrough, 
                    routingTargetType
                );
                Assert.Equal(4, strategy.Count());

                foreach (var impl in strategy) {
                    impl.ImplementOn(target);
                }

                foreach (var impl in strategy) {
                    Assert.True(target.ImplementInterface(impl.InterfaceType));
                }
            }
        }

        [Fact]
        public void SupportImplementationOfInterfacesInExternalAssembly() {
            
            using (var m = TestUtilities.GetModuleOfCurrentAssemblyForRewriting()) {
                var module = m.Module;
                
                var context = new CodeEmissionContext(module);
                var target = module.DefinitionOf(typeof(ClassThatWillReceiveRoutingInterfaceFromExternalAssembly));

                var externalInterface = ProviderOfExternalRoutingTypes.RoutingInterface;
                var passThroughType = ProviderOfExternalRoutingTypes.DbProxyInterface;
                var externalTargetType = ProviderOfExternalRoutingTypes.RoutingTargetType;
                
                var externalImplementation = new RoutedInterfaceImplementation(context, externalInterface, passThroughType, externalTargetType);
                externalImplementation.ImplementOn(target);
            }
        }
    }
}
