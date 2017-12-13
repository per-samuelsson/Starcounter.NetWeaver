
using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Starcounter.Weaver {

    public static class MethodCallFinder {
        
        /// <summary>
        /// Analyze the body of the given method for a single call to any of the
        /// specified targets.
        /// </summary>
        /// <param name="caller">The caller method, possibly including a
        /// call.</param>
        /// <param name="methodTargets">All targets that should be considered.</param>
        /// <returns>Return the instruction if found, or null of no such call exist. 
        /// Raise exception if multiple calls are found.</returns>
        public static Instruction FindSingleCallToAnyTarget(MethodDefinition caller, IEnumerable<MethodReference> methodTargets) {
            Guard.NotNull(caller, nameof(caller));
            Guard.NotNull(methodTargets, nameof(methodTargets));
            if (!caller.HasBody) {
                throw new ArgumentException($"Call not found: method {caller.Name} has no body", nameof(caller));
            }

            return caller.Body.Instructions.SingleOrDefault(i => IsCallToAnyOfTargets(i, methodTargets));
        }
        
        static bool IsCallToAnyOfTargets(Instruction instruction, IEnumerable<MethodReference> targets) {
            return targets.Any(m => instruction.IsMethodCall(m));
        }
    }
}