
using System;

namespace Starcounter.Weaver.Tests.ExternalCode {

    public static class ProviderOfExternalRoutingTypes {

        public static Type DbProxyInterface {
            get {
                return typeof(IDbProxyInExternalAssembly);
            }
        }

        public static Type RoutingInterface {
            get {
                return typeof(IRoutedInterfaceInExternalCode);
            }
        }

        public static Type RoutingTargetType {
            get {
                return typeof(RoutedInterfaceTargetInExternalCode);
            }
        }
    }
}