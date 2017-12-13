
using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Linq;

namespace Starcounter.Weaver {

    public static class MethodBodyAnalysisExtensionMethods {
        static OpCode[] callInstructions = new[] { OpCodes.Call, OpCodes.Callvirt, OpCodes.Calli };
        
        public static bool IsMethodCall(this Instruction instruction, MethodReference method = null) {
            var result = callInstructions.Contains(instruction.OpCode);

            if (result && method != null) {
                var callee = instruction.Operand as MethodReference;
                result = callee == null ? false : callee.ReferenceSameMethod(method);
            }

            return result;
        }
    }
}
