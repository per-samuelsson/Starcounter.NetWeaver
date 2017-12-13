
namespace Starcounter.Weaver.Tests.ExternalCode {
    public class ProxyConstructorParameterTypeExternal {
    }

    public class ClassWithExplicitProxyConstructor {

        // Dummy to allow empty derived classes to compile
        public ClassWithExplicitProxyConstructor() { }

        public ClassWithExplicitProxyConstructor(ProxyConstructorParameterTypeExternal x) { }
    }
}
