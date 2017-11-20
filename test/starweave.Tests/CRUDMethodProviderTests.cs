﻿
using Starcounter2.Internal;
using System.Collections.Generic;
using Xunit;
using System.Linq;

namespace starweave.Tests {

    public class CRUDMethodProviderTests {

        class CustomCrudMethodProvider : DefaultCRUDMethodProvider {

            public override Dictionary<string, string> ReadMethods {
                get {
                    return new Dictionary<string, string>() {
                        { "System.Int", "Fake" },
                        { "System.Object", "Fake" }
                    };
                }
            }

            public override Dictionary<string, string> UpdateMethods {
                get {
                    return new Dictionary<string, string>() {
                        { "System.Int", "Fake" },
                        { "System.String", "Fake" },
                        { "System.Char", "Fake" }
                    };
                }
            }
        }

        [Fact]
        public void SupportedDataTypesAreUnionOfCRUDMethods() {
            var provider = new CustomCrudMethodProvider();
            Assert.Equal(2, provider.ReadMethods.Count());
            Assert.Equal(3, provider.UpdateMethods.Count());
            Assert.Equal(4, provider.SupportedDataTypes.Count());
        }
    }
}