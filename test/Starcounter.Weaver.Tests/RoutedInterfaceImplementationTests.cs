using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Starcounter.Weaver.Tests {

    interface IInterfaceWithSingleVoidMethod {
        void Method1();
    }

    interface IPassThroughType {}

    static class RoutingTargetType {

        public static void Method1(IPassThroughType t) {

        }
    }

    class EmptyType : IPassThroughType {

    }
    
    class EmptyType_Facit : IPassThroughType, IInterfaceWithSingleVoidMethod {

        void IInterfaceWithSingleVoidMethod.Method1() {
            RoutingTargetType.Method1(this);
        }
    }
    
    class WrongPassThroughType { }

    public class RoutedInterfaceImplementationTests {

        [Fact]
        public void BadInputGiveMeaningfulErrors() {
            var module = TestUtilities.GetModuleOfCurrentAssembly();

            var emitContext = new CodeEmissionContext(module);
            var interfaceDefinition = module.Types.Single(t => t.FullName == typeof(IInterfaceWithSingleVoidMethod).FullName);
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
        public void ImplementedInterfaceShowsUpAsExpected() {

            using (var m = TestUtilities.GetModuleOfCurrentAssemblyForRewriting()) {
                var module = m.Module;

                var interfaceDefinition = module.Types.Single(t => t.FullName == typeof(IInterfaceWithSingleVoidMethod).FullName);
                var passThrough = module.Types.Single(t => t.FullName == typeof(IPassThroughType).FullName);
                var routingTargetType = module.Types.Single(t => t.FullName == typeof(RoutingTargetType).FullName);
                
                var implementation = new RoutedInterfaceImplementation(new CodeEmissionContext(module), interfaceDefinition, passThrough, routingTargetType);

                var target = module.Types.Single(t => t.FullName == typeof(EmptyType).FullName);
                Assert.False(target.ImplementInterface(interfaceDefinition));
                implementation.ImplementOn(target);
                Assert.True(target.ImplementInterface(interfaceDefinition));
            }
        }
    }
}
