
using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Starcounter.Weaver.Rewriting {

    public static class RewritingAssertionMethods {

        public static void VerifyExpectedOriginalGetter(MethodDefinition getter) {
            Guard.NotNull(getter, nameof(getter));
            VerifyBodyHasExpectedSequence(getter, new[] {
                OpCodes.Ldarg_0,
                OpCodes.Ldfld,
                OpCodes.Ret
            });
        }

        public static void VerifyExpectedOriginalSetter(MethodDefinition setter) {
            Guard.NotNull(setter, nameof(setter));
            VerifyBodyHasExpectedSequence(setter, new[] {
                OpCodes.Ldarg_0,
                OpCodes.Ldarg_1,
                OpCodes.Stfld,
                OpCodes.Ret
            });
        }

        public static void VerifyExpectedReadMethod(AutoImplementedProperty property, MethodReference method) {
            var module = method.Module;
            var typeSystem = module.TypeSystem;
            var pass = !method.HasThis;
            if (pass) {
                pass = method.HasParameters && method.Parameters.Count == 3;
                if (pass) {
                    pass = method.Parameters.All(p => p.ParameterType.FullName.Equals(typeSystem.UInt64.FullName));
                }
            }

            if (!pass) {
                throw new ArgumentException($"Read method {method} dont have the expected signature.");
            }
        }

        public static void VerifyExpectedWriteMethod(AutoImplementedProperty property, MethodReference method) {
            var module = method.Module;
            var typeSystem = module.TypeSystem;
            var pass = !method.HasThis;
            if (pass) {
                pass = method.HasParameters && method.Parameters.Count == 4;
                if (pass) {
                    pass = method.Parameters.Take(3).All(p => p.ParameterType.FullName.Equals(typeSystem.UInt64.FullName));
                    if (pass) {
                        pass = method.MethodReturnType.ReturnType.FullName.Equals(typeSystem.Void.FullName);
                    }
                }
            }

            if (!pass) {
                throw new ArgumentException($"Write method {method} dont have the expected signature.");
            }
        }

        public static bool InstructionsHasExactSequence(IEnumerable<Instruction> instructions, IEnumerable<OpCode> codes) {
            Guard.NotNull(instructions, nameof(instructions));
            Guard.NotNull(codes, nameof(codes));

            var first = instructions.ToArray();
            var second = codes.ToArray();

            if (first.Length != second.Length) {
                return false;
            }

            for (int i = 0; i < first.Length; i++) {
                if (first[i].OpCode != second[i]) {
                    return false;
                }
            }

            return true;
        }

        static void VerifyBodyHasExpectedSequence(MethodDefinition method, OpCode[] sequence) {
            if (!method.HasBody) {
                throw new ArgumentException($"Method {method.Name} has no body", nameof(method));
            }

            var instructions = method.Body.Instructions.Where(i => i.OpCode != OpCodes.Nop);

            var match = InstructionsHasExactSequence(instructions, sequence);
            if (!match) {
                throw new ArgumentException($"Body of method {method.Name} does not match expected sequence");
            }
        }
    }
}
