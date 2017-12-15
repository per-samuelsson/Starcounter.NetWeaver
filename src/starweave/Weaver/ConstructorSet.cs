
using Mono.Cecil;
using Starcounter.Weaver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace starweave.Weaver {

    public sealed class ConstructorSet {

        public MethodDefinition ProxyConstructor { get; }

        public MethodDefinition InsertConstructor { get; }

        public IEnumerable<MethodDefinition> OriginalConstructors { get; }

        public IEnumerable<MethodDefinition> ReplacementConstructors { get; }

        public bool ContainConstructorsWithSignatureTypes {
            get {
                return ProxyConstructor != null || InsertConstructor != null || ReplacementConstructors.Count() > 0;
            }
        }

        public IEnumerable<MethodDefinition> All {
            get {
                var all = new List<MethodDefinition>();
                if (ProxyConstructor != null) {
                    all.Add(ProxyConstructor);
                }
                if (InsertConstructor != null) {
                    all.Add(InsertConstructor);
                }
                all.AddRange(OriginalConstructors);
                all.AddRange(ReplacementConstructors);

                return all;
            }
        }

        private ConstructorSet(MethodDefinition proxy, MethodDefinition insert, IEnumerable<MethodDefinition> replacements, IEnumerable<MethodDefinition> originals) {
            Guard.NotNull(replacements, nameof(replacements));
            Guard.NotNull(originals, nameof(originals));

            if (replacements.Count() > originals.Count()) {
                throw new ArgumentException("There can never be more replacement constructors than originals");
            }

            ProxyConstructor = proxy;
            InsertConstructor = insert;
            OriginalConstructors = originals;
            ReplacementConstructors = replacements;
        }

        public static ConstructorSet Discover(ConstructorSignatureTypes signatureTypes, TypeDefinition type) {
            return Discover(signatureTypes, type.GetInstanceConstructors());
        }

        public static ConstructorSet Discover(ConstructorSignatureTypes signatureTypes, IEnumerable<MethodDefinition> constructors) {
            MethodDefinition proxy = null;
            MethodDefinition insert = null;
            var replacements = new List<MethodDefinition>();
            var originals = new List<MethodDefinition>();

            foreach (var ctor in constructors) {
                if (!ctor.HasParameters) {
                    originals.Add(ctor);
                    continue;
                }

                var signatureParameter = ctor.Parameters.FirstOrDefault(p => signatureTypes.Types.Any(st => st.ReferenceSameType(p.ParameterType)));
                if (signatureParameter == null) {
                    originals.Add(ctor);
                }
                else if (ctor.Parameters.Count == 1) {
                    proxy = ctor;
                }
                else if (signatureParameter.ParameterType.ReferenceSameType(signatureTypes.InsertConstructorParameter)) {
                    insert = ctor;
                }
                else {
                    replacements.Add(ctor);
                }
            }

            return new ConstructorSet(proxy, insert, replacements, originals);
        }

        public IDictionary<MethodDefinition, MethodDefinition> GetReplacementMap() {
            var result = new Dictionary<MethodDefinition, MethodDefinition>(OriginalConstructors.Count());
            foreach (var original in OriginalConstructors) {
                // We support case where replacement constructor is not found.
                // This class should be usable on types where replacement constructors
                // have not yet been emitted.
                var replacement = ReplacementConstructors.SingleOrDefault(ctor => ReplacementConstructorEmitter.IsReplacementFor(ctor, original));
                result.Add(original, replacement);
            }
            return result;
        }
    }
}
