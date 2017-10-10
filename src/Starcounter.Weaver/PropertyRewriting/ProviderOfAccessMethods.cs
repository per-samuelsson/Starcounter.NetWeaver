using Mono.Cecil;

namespace Starcounter.Weaver.PropertyRewriting {
    public abstract class ProviderOfAccessMethods {
        public abstract MethodReference ProvideReadMethod(TypeReference dataType);
        public abstract MethodReference ProvideWriteMethod(TypeReference dataType);
    }
}
