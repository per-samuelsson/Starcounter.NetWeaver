
using Mono.Cecil;
using Starcounter.Weaver;
using starweave.Weaver;
using starweave.Weaver.Tests;
using System;
using System.Linq;
using Xunit;

namespace starweave.Tests {

    class ReplaceUniqueType { }

    class ReplaceCtorTestNoneDefined { }

    class ReplaceCtorTestCustomConstructors {

        public ReplaceCtorTestCustomConstructors(int i, string s) {

        }

        public ReplaceCtorTestCustomConstructors(ReplaceCtorTestCustomConstructors other) {

        }

        public ReplaceCtorTestCustomConstructors(bool b, ReplaceCtorTestCustomConstructors other, int i) {

        }

        public ReplaceCtorTestCustomConstructors(DateTime dt, int? nullableint) {

        }

        private ReplaceCtorTestCustomConstructors() {

        }

        protected ReplaceCtorTestCustomConstructors(object o, decimal d, Guid guid) {

        }
    }

    public class ReplacementConstructorEmitterTests {

        [Fact]
        public void EmitGenerateReplacementConstructorWithSimilarTraitsForDefaultConstructor() {
            using (var moduleHandle = TestUtilities.GetModuleOfCurrentAssemblyForRewriting()) {
                var module = moduleHandle.Module;

                var uniqueType = module.Types.Single(t => t.FullName == typeof(ReplaceUniqueType).FullName);
                var targetType = module.Types.Single(t => t.FullName == typeof(ReplaceCtorTestNoneDefined).FullName);
                var original = targetType.Methods.Single(m => m.Name == ".ctor");

                var emitter = new ReplacementConstructorEmitter(new CodeEmissionContext(module), uniqueType);
                var replacement = emitter.Emit(original);
                AssertExpectedReplacement(original, replacement);
            }
        }

        [Fact]
        public void EmitGenerateReplacementConstructorWithSimilarTraitsForCustomConstructors() {
            using (var moduleHandle = TestUtilities.GetModuleOfCurrentAssemblyForRewriting()) {
                var module = moduleHandle.Module;

                var uniqueType = module.Types.Single(t => t.FullName == typeof(ReplaceUniqueType).FullName);
                var targetType = module.Types.Single(t => t.FullName == typeof(ReplaceCtorTestCustomConstructors).FullName);
                var originals = targetType.Methods.Where(m => m.Name == ".ctor");

                var emitter = new ReplacementConstructorEmitter(new CodeEmissionContext(module), uniqueType);
                var arr = originals.ToArray();

                foreach (var original in arr) {
                    var replacement = emitter.Emit(original);
                    AssertExpectedReplacement(original, replacement);
                }
            }
        }

        void AssertExpectedReplacement(MethodDefinition original, MethodDefinition replacement) {
            Assert.NotNull(replacement);
            Assert.True(replacement.DeclaringType.FullName.Equals(original.DeclaringType.FullName));
            Assert.Equal(replacement.Body.Instructions.Count, original.Body.Instructions.Count);
            Assert.Equal(replacement.Parameters.Count, original.Parameters.Count + 2);
        }
    }
}