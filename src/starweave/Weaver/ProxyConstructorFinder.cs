
using Mono.Cecil;
using Starcounter.Weaver;
using System;
using System.Linq;

namespace starweave.Weaver {

    public class ProxyConstructorFinder {
        protected readonly TypeReference signatureType;

        public ProxyConstructorFinder(TypeReference signatureTypeReference) {
            signatureType = signatureTypeReference ?? throw new ArgumentNullException(nameof(signatureTypeReference));
        }

        public MethodReference FindProxyConstructor(TypeDefinition type) {
            return type.GetInstanceConstructors().FirstOrDefault(c => {
                return HasProxyConstructorSignature(c, signatureType);
            });
        }

        public static bool HasProxyConstructorSignature(MethodDefinition ctor, TypeReference signatureType) {
            return ctor.HasParameters && ctor.Parameters.Count == 1 && ctor.Parameters[0].ParameterType.ReferenceSameType(signatureType);
        }
    }
}