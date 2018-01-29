
namespace Starcounter.Weaver.Tests.ExternalCode {

    public static class RoutedInterfaceTargetInExternalCode {

        public static ulong Test(IDbProxyInExternalAssembly passThrough) {
            return 42;
        }

        public static bool TestWithExternalParameter(IDbProxyInExternalAssembly passThrough, ExternalParameterType t) {
            return true;
        }
    }
}
