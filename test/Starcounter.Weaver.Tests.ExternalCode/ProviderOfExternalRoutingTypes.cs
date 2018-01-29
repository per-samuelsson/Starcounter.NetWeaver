
using System;

namespace Starcounter.Weaver.Tests.ExternalCode {

    public static class ProviderOfExternalRoutingTypes {

        public static Type DbProxyInterface {
            get {
                return typeof(IDbProxyInExternalAssembly);
            }
        }
    }
}