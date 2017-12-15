using Mono.Cecil;
using Starcounter.Weaver;
using starweave.Weaver;
using starweave.Weaver.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace starweave.Tests {

    public class NoBaseNoDefinedConstructor { }
    public sealed class MockProxyParameter { }
    public sealed class MockInsertParameter { }

    public class DatabaseTypeConstructorRewriterTests {

        public void MockInsertMethod(out ulong x, out ulong y, ulong z) {
            x = y = 0;
        }

        [Fact]
        public void NoBaseNoCtorProduceExpectedConstructorSet() {

            using (var mod = TestUtilities.GetModuleOfCurrentAssemblyForRewriting()) {
                var module = mod.Module;

                var type = module.Types.Single(t => t.FullName == typeof(NoBaseNoDefinedConstructor).FullName);
                var originalCtors = type.GetInstanceConstructors();
                var defaultCtor = originalCtors.Single();

                var testType = module.Types.Single(t => t.FullName == typeof(DatabaseTypeConstructorRewriterTests).FullName);

                var mockInsert = typeof(DatabaseTypeConstructorRewriterTests).GetMethod(nameof(DatabaseTypeConstructorRewriterTests.MockInsertMethod));
                var emitContext = new CodeEmissionContext(module);

                var state = new DatabaseTypeStateEmitter(emitContext, type, new DatabaseTypeStateNames());
                state.EmitReferenceFields();
                state.EmitCRUDHandles();

                var rewriter = new DatabaseTypeConstructorRewriter(
                    new DefaultWeaverHost(TestUtilities.QuietDiagnostics),
                    emitContext,
                    typeof(MockProxyParameter),
                    typeof(MockInsertParameter),
                    mockInsert);
                rewriter.Rewrite(type, null, state);

                AssertExpectedConstructorCountAfterRewrite(1, type, null);

                var constructors = ConstructorSet.Discover(
                    new ConstructorSignatureTypes(emitContext, typeof(MockProxyParameter), typeof(MockInsertParameter)), type);

                Assert.NotNull(constructors.ProxyConstructor);
                Assert.NotNull(constructors.InsertConstructor);
                Assert.Equal(1, constructors.OriginalConstructors.Count());
                Assert.Equal(1, constructors.ReplacementConstructors.Count());
            }
        }

        static void AssertExpectedConstructorCountAfterRewrite(int countBefore, TypeDefinition type, TypeDefinition baseType) {
            // For each constructor found, there is one extra.
            // Then the proxy constructor.
            // Then, for roots, the insert constructor.

            // For roots:
            // 1 = 1 + 1 + 1 = 3 added.
            // 2 = 2 + 1 + 1 = 4 added.

            // For derived:
            // 1 = 1 + 1 = 2 added
            // 2 = 2 + 1 = 3 added

            var expected = countBefore * 2;
            expected++; // Proxy
            if (baseType == null) {
                expected++; // Insert
            }

            Assert.Equal(expected, type.GetInstanceConstructors().Count());
        }
    }
}