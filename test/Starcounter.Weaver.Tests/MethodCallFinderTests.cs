
using Mono.Cecil;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using Xunit;

namespace Starcounter.Weaver.Tests {

    public class MethodCallFinderTests {

        [DllImport("dummy.dll")]
        public static extern int MethodWithNoBody();

        public void MethodWithNoCall() {}

        public void MethodWithOneCall() {
            MethodWithNoCall();
        }

        public bool MethodWithTwoCalls() {
            MethodWithNoCall();
            MethodWithNoCall();
            return true;
        }

        [Fact]
        public void BadInputProduceMeaningfulErrors() {
            var module = TestUtilities.GetModuleOfCurrentAssembly();

            // Using Single here will assure there is a single method as we
            // expect. Hence, no assertions need to be made.
            var type = module.Types.Single(t => t.FullName == typeof(MethodCallFinderTests).FullName);
            var nobody = type.Methods.Single(m => m.Name == nameof(MethodWithNoBody));
            var nocall = type.Methods.Single(m => m.Name == nameof(MethodWithNoCall));
            
            var e = Assert.Throws<ArgumentNullException>(() => MethodCallFinder.FindSingleCallToAnyTarget(null, new MethodReference[0]));
            Assert.Equal("caller", e.ParamName);

            var ae = Assert.Throws<ArgumentException>(() => MethodCallFinder.FindSingleCallToAnyTarget(nobody, new MethodReference[0]));
            Assert.Equal("caller", e.ParamName);

            e = Assert.Throws<ArgumentNullException>(() => MethodCallFinder.FindSingleCallToAnyTarget(nocall, null));
            Assert.Equal("methodTargets", e.ParamName);
        }

        [Fact]
        public void MethodsWithNoCallRenderNoHit() {
            var module = TestUtilities.GetModuleOfCurrentAssembly();

            // Using Single here will assure there is a single method as we
            // expect. Hence, no assertions need to be made.
            var type = module.Types.Single(t => t.FullName == typeof(MethodCallFinderTests).FullName);
            var nocall = type.Methods.Single(m => m.Name == nameof(MethodWithNoCall));

            var instruction = MethodCallFinder.FindSingleCallToAnyTarget(nocall, new [] { nocall });
            Assert.Null(instruction);
        }

        [Fact]
        public void SingleCallLookupRaiseErrorOnMultipleCalls() {
            var module = TestUtilities.GetModuleOfCurrentAssembly();

            // Using Single here will assure there is a single method as we
            // expect. Hence, no assertions need to be made.
            var type = module.Types.Single(t => t.FullName == typeof(MethodCallFinderTests).FullName);
            var nocall = type.Methods.Single(m => m.Name == nameof(MethodWithNoCall));
            var twocalls = type.Methods.Single(m => m.Name == nameof(MethodWithTwoCalls));
            
            Assert.Throws<InvalidOperationException>(() => MethodCallFinder.FindSingleCallToAnyTarget(twocalls, new[] { nocall }));
        }

        [Fact]
        public void SingleCallLookupHitWithExpectedOperand() {
            var module = TestUtilities.GetModuleOfCurrentAssembly();

            // Using Single here will assure there is a single method as we
            // expect. Hence, no assertions need to be made.
            var type = module.Types.Single(t => t.FullName == typeof(MethodCallFinderTests).FullName);
            var nocall = type.Methods.Single(m => m.Name == nameof(MethodWithNoCall));
            var onecall = type.Methods.Single(m => m.Name == nameof(MethodWithOneCall));
            
            var instruction = MethodCallFinder.FindSingleCallToAnyTarget(onecall, new[] { nocall });
            Assert.NotNull(instruction);
            var operand = instruction.Operand as MethodReference;
            Assert.NotNull(operand);
            Assert.True(operand.ReferenceSameMethod(nocall));
        }
    }
}
