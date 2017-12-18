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

    public sealed class MockProxyParameter { }
    public sealed class MockInsertParameter { }

    public class NoBaseNoDefinedConstructor { }

    public class DerivedSameAssemblyNoConstructor : NoBaseNoDefinedConstructor { }
    
    public class DatabaseTypeConstructorRewriterTests {

        public void MockInsertMethod(out ulong x, out ulong y, ulong z) {
            x = y = 0;
        }
        
        [Fact]
        public void NoBaseNoCtorProduceExpectedConstructorSet() {

            using (var mod = TestUtilities.GetModuleOfCurrentAssemblyForRewriting()) {
                var module = mod.Module;

                // mod.OutputStream = System.IO.File.Create(@"c:\Users\Per\test.dll");

                var type = module.Types.Single(t => t.FullName == typeof(NoBaseNoDefinedConstructor).FullName);

                DatabaseTypeState state;
                DatabaseTypeConstructorRewriter rewriter;
                ConstructorSignatureTypes signatures;
                CreateRewritingContext(type, out state, out signatures, out rewriter);

                var originalCtors = type.GetInstanceConstructors();
                Assert.NotNull(originalCtors.SingleOrDefault());
                
                rewriter.Rewrite(type, null, state);

                AssertExpectedConstructorCountAfterRewrite(1, type, null);

                var constructors = ConstructorSet.Discover(signatures, type);

                AssertFullConstructorSet(constructors, 1);
            }
        }

        [Fact]
        public void NoBaseNoCtorRenderConstructorsWhereNoCallToOriginalOnesRemain() {

            using (var mod = TestUtilities.GetModuleOfCurrentAssemblyForRewriting()) {
                var module = mod.Module;
                
                var type = module.Types.Single(t => t.FullName == typeof(NoBaseNoDefinedConstructor).FullName);

                DatabaseTypeState state;
                DatabaseTypeConstructorRewriter rewriter;
                ConstructorSignatureTypes signatures;
                CreateRewritingContext(type, out state, out signatures, out rewriter);

                rewriter.Rewrite(type, null, state);
                
                var constructors = ConstructorSet.Discover(signatures, type);

                AssertFullConstructorSet(constructors, 1);

                foreach (var ctor in constructors.All) {
                    var call = MethodCallFinder.FindSingleCallToAnyTarget(ctor, constructors.OriginalConstructors);
                    Assert.Null(call);
                }
            }
        }

        [Fact]
        public void DerivedSameAssemblyNoCtorRenderConstructorsWhereNoCallToOriginalOnesRemain() {

            using (var mod = TestUtilities.GetModuleOfCurrentAssemblyForRewriting()) {
                var module = mod.Module;

                var baseType = module.Types.Single(t => t.FullName == typeof(NoBaseNoDefinedConstructor).FullName);

                DatabaseTypeState state;
                DatabaseTypeConstructorRewriter rewriter;
                ConstructorSignatureTypes signatures;
                CreateRewritingContext(baseType, out state, out signatures, out rewriter);

                rewriter.Rewrite(baseType, null, state);
                var baseConstructors = ConstructorSet.Discover(signatures, baseType);
                AssertFullConstructorSet(baseConstructors, 1);

                var type = module.Types.Single(t => t.FullName == typeof(DerivedSameAssemblyNoConstructor).FullName);
                CreateRewritingContext(type, out state, out signatures, out rewriter);

                rewriter.Rewrite(type, baseType, state);
                var constructors = ConstructorSet.Discover(signatures, type);

                var originals = new List<MethodDefinition>(baseConstructors.OriginalConstructors);
                originals.AddRange(constructors.OriginalConstructors);

                foreach (var ctor in constructors.All) {
                    var call = MethodCallFinder.FindSingleCallToAnyTarget(ctor, originals);
                    Assert.Null(call);
                }
            }
        }

        static void CreateRewritingContext(
            TypeDefinition type,
            out DatabaseTypeState state,
            out ConstructorSignatureTypes signatures,
            out DatabaseTypeConstructorRewriter rewriter
            ) {

            var module = type.Module;
            var mockInsertMethod = typeof(DatabaseTypeConstructorRewriterTests).GetMethod(nameof(DatabaseTypeConstructorRewriterTests.MockInsertMethod));
            var emitContext = new CodeEmissionContext(module);

            var stateEmit = new DatabaseTypeStateEmitter(emitContext, type, new DatabaseTypeStateNames());
            stateEmit.EmitReferenceFields();
            stateEmit.EmitCRUDHandles();
            state = stateEmit;

            rewriter = new DatabaseTypeConstructorRewriter(
                new DefaultWeaverHost(TestUtilities.QuietDiagnostics),
                emitContext,
                typeof(MockProxyParameter),
                typeof(MockInsertParameter),
                mockInsertMethod
            );

            signatures = new ConstructorSignatureTypes(emitContext, typeof(MockProxyParameter), typeof(MockInsertParameter));
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

        static void AssertFullConstructorSet(ConstructorSet constructors, int originalConstructorCount = -1) {
            Assert.NotNull(constructors.ProxyConstructor);
            Assert.NotNull(constructors.InsertConstructor);
            Assert.NotEmpty(constructors.OriginalConstructors);
            Assert.NotEmpty(constructors.ReplacementConstructors);
            Assert.Equal(constructors.ReplacementConstructors.Count(), constructors.OriginalConstructors.Count());
            if (originalConstructorCount > 0) {
                Assert.Equal(originalConstructorCount, constructors.OriginalConstructors.Count());
            }
        }
    }
}