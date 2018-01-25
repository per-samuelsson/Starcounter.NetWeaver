using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Text;

namespace Starcounter.Weaver {

    /// <summary>
    /// Simple compararer that can be used to compare specialization level of
    /// types.
    /// </summary>
    public class TypeSpecializationComparer : IComparer<TypeDefinition> {

        public int Compare(TypeDefinition x, TypeDefinition y) {
            Guard.NotNull(x, nameof(x));
            Guard.NotNull(y, nameof(y));
            
            if (x == y || x.MetadataToken == y.MetadataToken) {
                return 0;
            }

            if (x.IsAssignableFrom(y)) {
                return -1;
            }
            else if (y.IsAssignableFrom(x)) {
                return 1;
            }

            return 0;
        }
    }
}