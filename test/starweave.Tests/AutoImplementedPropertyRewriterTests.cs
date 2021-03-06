﻿
using Starcounter.Weaver;
using Starcounter.Weaver.Runtime;
using System.Linq;
using Xunit;

namespace starweave.Weaver.Tests {

    public class ClassWithIntAutoProperty {

        public int Int1 { get; set; }

        public static int GetInt(ulong i1, ulong i2, ulong i3) {
            return 42;
        }

        public static void SetInt(ulong i1, ulong i2, ulong i3, int v) {
        }
    }
    
    public class AutoImplementedPropertyRewriterTests {

        [Fact]
        public void AllPropertiesOfIntAutoPropertiesHasExpectedImplementation() {
            var module = TestUtilities.GetModuleOfCurrentAssembly();

            var type = module.Types.Single(t => t.FullName == typeof(IntAutoProperties).FullName);
            Assert.NotNull(type);

            Assert.True(type.Properties.All(p => {
                RewritingAssertionMethods.VerifyExpectedOriginalGetter(p.GetMethod);
                if (p.SetMethod != null) {
                    RewritingAssertionMethods.VerifyExpectedOriginalSetter(p.SetMethod);
                }
                return true;
            }));
        }

        [Fact]
        public void RewriterAcceptStateWithReferenceFields() {

            using (var m = TestUtilities.GetModuleOfCurrentAssemblyForRewriting()) {
                var module = m.Module;

                var type = module.Types.Single(t => t.FullName == typeof(ClassWithIntAutoProperty).FullName);
                Assert.NotNull(type);

                var context = new CodeEmissionContext(module);
                var state = new DatabaseTypeStateEmitter(new CodeEmissionContext(module), type, new DatabaseTypeStateNames());
                state.EmitReferenceFields();

                new AutoImplementedPropertyRewriter(context, state);
            }
        }

        [Fact]
        public void RewritingIntProperty() {

            using (var mod = TestUtilities.GetModuleOfCurrentAssemblyForRewriting()) {
                var module = mod.Module;

                var type = module.Types.Single(t => t.FullName == typeof(ClassWithIntAutoProperty).FullName);
                Assert.NotNull(type);

                var property = type.Properties.Single(p => p.Name == nameof(ClassWithIntAutoProperty.Int1));
                var readMethod = type.Methods.Single(m => m.Name == nameof(ClassWithIntAutoProperty.GetInt));
                var writeMethod = type.Methods.Single(m => m.Name == nameof(ClassWithIntAutoProperty.SetInt));

                var context = new CodeEmissionContext(module);
                var state = new DatabaseTypeStateEmitter(new CodeEmissionContext(module), type, new DatabaseTypeStateNames());
                state.EmitReferenceFields();
                state.EmitPropertyCRUDHandle(nameof(ClassWithIntAutoProperty.Int1));

                var autoProperty = new AutoImplementedProperty(property);
                var rewriter = new AutoImplementedPropertyRewriter(context, state);

                rewriter.Rewrite(autoProperty, readMethod, writeMethod);
            }
        }
    }
}