using System;
using System.Linq;
using Xunit;

namespace Starcounter.Weaver.Tests {

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
    }
}
