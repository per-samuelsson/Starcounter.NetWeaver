
using Starcounter.Weaver.Rewriting;
using System;
using System.Linq;
using Xunit;

namespace Starcounter.Weaver.Tests {

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
    }
}