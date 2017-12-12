
using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Linq;

namespace Starcounter.Weaver {

    public static class MethodBodyAnalysisExtensionMethods {
        static OpCode[] callInstructions = new[] { OpCodes.Call, OpCodes.Callvirt, OpCodes.Calli };

        public static bool ReferenceSameMethod(MethodReference reference, MethodReference other) {
            // Meta tokens aren't accurate: definition will have a different token than
            // any reference. FullName is slow but at least functional here.
            return reference.FullName.Equals(other.FullName);
        }

        public static bool IsMethodCall(this Instruction instruction, MethodReference method = null) {
            var result = callInstructions.Contains(instruction.OpCode);

            if (result && method != null) {
                var callee = instruction.Operand as MethodReference;
                result = callee == null ? false : ReferenceSameMethod(callee, method);
            }

            return result;
        }
    }
}
