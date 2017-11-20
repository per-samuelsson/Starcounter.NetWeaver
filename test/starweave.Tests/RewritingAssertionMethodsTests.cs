
using Starcounter.Weaver;
using System;
using System.Linq;
using Xunit;

namespace starweave.Weaver.Tests {

    abstract class ClassWithAbstractMethod {

        public abstract void AbstractMethod();
    }

    class ClassWithExplicitProperties {
        int dummy;

        public int NotAutoImplemented {
            get {
                if (DateTime.Today.Month == 9) {
                    return 42;
                }
                return 42 + dummy;
            }
            set {
                if (value == 42) {
                    throw new ArgumentException();
                }
                dummy = value;
            }
        }
    }

    class InvalidReadMethods {

        int NotStatic(ulong i1, ulong i2, ulong i3) { return 42; }

        static int NoParameters() { return 42; }

        static int ToManyParameters(ulong i1, ulong i2, ulong i3, ulong i4) { return 42; }

        static int WrongParameterType(ulong i1, int i2, ulong i3) { return 42; }
    }

    class InvalidWriteMethods {

        void NotStatic(ulong i1, ulong i2, ulong i3, ulong u4) { }

        int ReturnValue(ulong i1, ulong i2, ulong i3, ulong u4) { return 42; }

        static void NoParameters() { }

        static void ToManyParameters(ulong i1, ulong i2, ulong i3, ulong i4, ulong i5) { }

        static void WrongParameterType(ulong i1, int i2, ulong i3, ulong i4) { }
    }

    class ProperReadAndWriteMethods {
        public static int ReadInt(ulong i1, ulong i2, ulong i3) { return 42; }
        public static void WriteInt(ulong i1, ulong i2, ulong i3, int value) {  }
    }

    public class RewritingAssertionMethodsTests {

        [Fact]
        public void BadInputReportMeaningfulErrors() {
            var module = TestUtilities.GetModuleOfCurrentAssembly();

            var test1 = module.Types.Single(t => t.FullName == typeof(ClassWithAbstractMethod).FullName);
            Assert.NotNull(test1);

            var abstractMethod = test1.Methods.Single(m => m.Name == nameof(ClassWithAbstractMethod.AbstractMethod));
            Assert.NotNull(abstractMethod);

            var npe = Assert.Throws<ArgumentNullException>(() => RewritingAssertionMethods.VerifyExpectedOriginalGetter(null));
            Assert.Equal("getter", npe.ParamName);
            npe = Assert.Throws<ArgumentNullException>(() => RewritingAssertionMethods.VerifyExpectedOriginalSetter(null));
            Assert.Equal("setter", npe.ParamName);

            var ae = Assert.Throws<ArgumentException>(() => RewritingAssertionMethods.VerifyExpectedOriginalSetter(abstractMethod));
            Assert.Contains("body", ae.Message);

            test1 = module.Types.Single(t => t.FullName == typeof(ClassWithExplicitProperties).FullName);
            Assert.NotNull(test1);

            var p = test1.Properties.Single(prop => prop.Name == nameof(ClassWithExplicitProperties.NotAutoImplemented));
            Assert.NotNull(p);

            ae = Assert.Throws<ArgumentException>(() => RewritingAssertionMethods.VerifyExpectedOriginalGetter(p.GetMethod));
            Assert.Contains("does not match expected sequence", ae.Message);

            ae = Assert.Throws<ArgumentException>(() => RewritingAssertionMethods.VerifyExpectedOriginalSetter(p.SetMethod));
            Assert.Contains("does not match expected sequence", ae.Message);
        }

        [Fact]
        public void IntPropertiesAreExpected() {
            var module = TestUtilities.GetModuleOfCurrentAssembly();

            var type = module.Types.Single(t => t.FullName == typeof(IntAutoProperties).FullName);
            Assert.NotNull(type);

            var properties = type.Properties;
            Assert.True(properties.All(p => p.IsAutoImplemented()));

            foreach (var p in properties) {
                var autoProp = new AutoImplementedProperty(p);
                var getter = autoProp.Property.GetMethod;
                var setter = autoProp.Property.SetMethod;

                RewritingAssertionMethods.VerifyExpectedOriginalGetter(getter);
                if (setter != null) {
                    RewritingAssertionMethods.VerifyExpectedOriginalSetter(setter);
                }
            }
        }

        [Fact]
        public void DetectInvalidReadMethods() {
            var module = TestUtilities.GetModuleOfCurrentAssembly();

            var invalidReadMethods = module.Types.Single(t => t.FullName == typeof(InvalidReadMethods).FullName);
            Assert.NotNull(invalidReadMethods);

            var intProperties = module.Types.Single(t => t.FullName == typeof(IntAutoProperties).FullName);
            Assert.NotNull(intProperties);

            var dummy = new AutoImplementedProperty(intProperties.Properties.First());
            foreach (var readMethod in invalidReadMethods.Methods) {
                var e = Assert.Throws<ArgumentException>(() => {
                    RewritingAssertionMethods.VerifyExpectedReadMethod(dummy, readMethod);
                });
            }
        }

        [Fact]
        public void DetectInvalidWriteMethods() {
            var module = TestUtilities.GetModuleOfCurrentAssembly();

            var invalidWriteMethods = module.Types.Single(t => t.FullName == typeof(InvalidWriteMethods).FullName);
            Assert.NotNull(invalidWriteMethods);

            var intProperties = module.Types.Single(t => t.FullName == typeof(IntAutoProperties).FullName);
            Assert.NotNull(intProperties);

            var dummy = new AutoImplementedProperty(intProperties.Properties.First());
            foreach (var writeMethod in invalidWriteMethods.Methods) {
                var e = Assert.Throws<ArgumentException>(() => {
                    RewritingAssertionMethods.VerifyExpectedWriteMethod(dummy, writeMethod);
                });
            }
        }

        [Fact]
        public void ProperReadAndWriteMethodsPassVerification() {
            var module = TestUtilities.GetModuleOfCurrentAssembly();

            var properReadWriteMethods = module.Types.Single(t => t.FullName == typeof(ProperReadAndWriteMethods).FullName);
            Assert.NotNull(properReadWriteMethods);

            var readInt = properReadWriteMethods.Methods.Single(m => m.Name == nameof(ProperReadAndWriteMethods.ReadInt));
            Assert.NotNull(readInt);

            var writeInt = properReadWriteMethods.Methods.Single(m => m.Name == nameof(ProperReadAndWriteMethods.WriteInt));
            Assert.NotNull(writeInt);

            var intProperties = module.Types.Single(t => t.FullName == typeof(IntAutoProperties).FullName);
            Assert.NotNull(intProperties);

            var dummy = new AutoImplementedProperty(intProperties.Properties.First());

            RewritingAssertionMethods.VerifyExpectedReadMethod(dummy, readInt);
            RewritingAssertionMethods.VerifyExpectedWriteMethod(dummy, writeInt);
        }
    }
}