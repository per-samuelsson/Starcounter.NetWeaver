
using Mono.Cecil;
using System;
using System.Reflection;

namespace Starcounter.Weaver {

    /// <summary>
    /// Provide any rewriting code an abstraction over Cecil
    /// reference importing until we have figured out how to best
    /// deal with it, including how duplicate imports behave and
    /// cost.
    /// </summary>
    public sealed class CodeEmissionContext {
        readonly ModuleDefinition module;

        public ModuleDefinition Module => module;

        public CodeEmissionContext(ModuleDefinition emissionTargetModule) {
            module = emissionTargetModule ?? throw new ArgumentNullException(nameof(emissionTargetModule));
        }

        public TypeReference Use(TypeReference type) {
            return module.ImportReference(type);
        }

        public TypeReference Use(Type type) {
            return module.ImportReference(type);
        }

        public MethodReference Use(MethodReference method) {
            return module.ImportReference(method);
        }

        public MethodReference Use(MethodInfo method) {
            return module.ImportReference(method);
        }

        public bool Defines(TypeDefinition type) {
            return type.Module == Module;
        }
    }
}
