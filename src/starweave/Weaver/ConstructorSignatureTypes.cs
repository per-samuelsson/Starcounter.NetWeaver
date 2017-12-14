
using Mono.Cecil;
using Starcounter.Weaver;
using System;
using System.Collections.Generic;

namespace starweave.Weaver {

    public class ConstructorSignatureTypes {

        public TypeReference ProxyConstructorParameter { get; }

        public TypeReference InsertConstructorParameter { get; }

        public IEnumerable<TypeReference> Types {
            get {
                return new[] { ProxyConstructorParameter, InsertConstructorParameter };
            }
        }

        public ConstructorSignatureTypes(CodeEmissionContext emitContext, Type proxyConstructorParameterType, Type insertConstructorParameterType) {
            ProxyConstructorParameter = emitContext.Use(proxyConstructorParameterType);
            InsertConstructorParameter = emitContext.Use(insertConstructorParameterType);
        }
    }
}