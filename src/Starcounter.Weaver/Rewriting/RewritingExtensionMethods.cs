
using Mono.Cecil;
using System.Linq;

namespace Starcounter.Weaver.Rewriting {

    // Internals here need to be syncronized with the host, that need
    // to assign these fields (at least handles).
    // TODO:

    static class RewritingExtensionMethods {

        public static FieldDefinition GetIdField(this TypeDefinition type) {
            return type.Fields.Single(f => f.Name.Equals("<star>_dbId"));
        }

        public static FieldDefinition GetRefField(this TypeDefinition type) {
            return type.Fields.Single(f => f.Name.Equals("<star>_dbRef"));
        }

        public static FieldDefinition GetPropertyHandleField(this PropertyDefinition prop) {
            var type = prop.DeclaringType;
            var name = $"<star>_{prop.Name}_propertyhandle";
            return type.Fields.Single(f => f.Name.Equals(name));
        }
    }
}
