
using Mono.Cecil;
using Starcounter.Weaver.Tests.ExternalCode;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Starcounter.Weaver.Tests {

    public class SingleCallMethodTargets {

        public void Method1() {

        }
    }

    public class SingleCallMethodsBase {

        protected SingleCallMethodsBase() {

        }

        protected SingleCallMethodsBase(string s) {

        }
    }

    /// <summary>
    /// Each public method contains a single call to some other method with
    /// some certain traits.
    /// </summary>
    public class SingleCallMethods : SingleCallMethodsBase {

        // Private not to be tested
        private void Method123() {
        }

        // Private not to be tested
        private SingleCallMethods() : base() {
        }

        public SingleCallMethods(int x) : this() {
        }

        public SingleCallMethods(string s) : base(s) {
        }

        public string CallToString(int i) {
            return i.ToString();
        }

        public bool CallToEquals(int i) {
            return i.Equals(42);
        }

        public void CallPrivate() {
            Method123();
        }

        public void CallInSameAssembly(SingleCallMethodTargets t) {
            t.Method1();
        }

        public void CallInAnotherAssembly(ClassWithMethods c) {
            c.Method1();
        }

        public Type CallInAnotherAssembly2(ClassWithMethods c) {
            return c.GetType();
        }
    }

    public class CtorCall1 {

        public CtorCall1() {

        }

        public CtorCall1(int x) : this() {

        }
    }

    public class SpecificCalls : ClassWithMethods {

        public SpecificCalls() : base(10) { }

        public void Target1() { }

        public void Target2_NotCalled() { }

        public void Call_Target1() {
            Target1();
        }
    }

    public class CtorCallsToBaseInExternalAssembly : ClassWithMethods {

        public CtorCallsToBaseInExternalAssembly() {}

        public CtorCallsToBaseInExternalAssembly(Guid guid, string s) : this(guid) { }

        public CtorCallsToBaseInExternalAssembly(Guid guid) : base("123-456-789A-BCDE") { }

        public CtorCallsToBaseInExternalAssembly(int i) : base(i) { }
    }

    public class MethodCallAnalysisTests {

        [Fact]
        public void SingleCallMethodsCanBeIdentified() {
            var module = TestUtilities.GetModuleOfCurrentAssembly();

            var singleCallMethods = new List<MethodDefinition>();
            var singleCallMethodsType = module.Types.Single(t => t.FullName == typeof(SingleCallMethods).FullName);
            singleCallMethods.AddRange(singleCallMethodsType.Methods.Where(m => m.IsPublic));
            singleCallMethodsType = module.Types.Single(t => t.FullName == typeof(CtorCallsToBaseInExternalAssembly).FullName);
            singleCallMethods.AddRange(singleCallMethodsType.Methods.Where(m => m.IsConstructor && m.IsPublic));

            Assert.NotEmpty(singleCallMethods);
            foreach (var singleCallMethod in singleCallMethods) {
                var instruction = singleCallMethod.Body.Instructions.Single(i => i.IsMethodCall());
                Assert.NotNull(instruction);
            }
        }

        [Fact]
        public void MethodCallsCanBeIdentifiedForGivenCallee() {
            var module = TestUtilities.GetModuleOfCurrentAssembly();

            var specificCallsType = module.Types.Single(t => t.FullName == typeof(SpecificCalls).FullName);
            var baseType = specificCallsType.BaseType.Resolve();
            Assert.NotNull(baseType);
            Assert.Equal(baseType.FullName, typeof(ClassWithMethods).FullName);

            var caller = specificCallsType.Methods.Single(m => m.Name == nameof(SpecificCalls.Call_Target1));
            var callee = specificCallsType.Methods.Single(m => m.Name == nameof(SpecificCalls.Target1));

            var call = caller.Body.Instructions.Single(i => i.IsMethodCall());
            Assert.True(call.IsMethodCall(callee));
            callee = specificCallsType.Methods.Single(m => m.Name == nameof(SpecificCalls.Target2_NotCalled));
            Assert.False(call.IsMethodCall(callee));

            caller = specificCallsType.Methods.Single(m => m.IsConstructor && m.Parameters.Count == 0);
            callee = baseType.Methods.Single(m => m.IsConstructor && m.Parameters.Count == 1 && m.Parameters[0].ParameterType.FullName == typeof(int).FullName);
            call = caller.Body.Instructions.Single(i => i.IsMethodCall());
            Assert.True(call.IsMethodCall(callee));
        }
    }
}
