
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Starcounter.Weaver {

    public class RoutedMethodImplementation {
        readonly InterfaceImplementation interfaceImplementation;
        readonly MethodDefinition interfaceMethod;
        readonly MethodDefinition targetMethod;
        
        public MethodDefinition ImplementedMethod {
            get;
            private set;
        }

        public RoutedMethodImplementation(InterfaceImplementation implementation, MethodDefinition interfaceDefinition, MethodDefinition targetDefinition) {
            Guard.NotNull(implementation, nameof(implementation));
            Guard.NotNull(interfaceDefinition, nameof(interfaceDefinition));
            Guard.NotNull(targetDefinition, nameof(targetDefinition));
            
            interfaceMethod = interfaceDefinition;
            targetMethod = targetDefinition;
            interfaceImplementation = implementation;
        }

        public void ImplementOn(TypeDefinition type) {
            var name = interfaceImplementation.InterfaceType.FullName + "." + interfaceMethod.Name;
            var attributes = interfaceMethod.Attributes;
            attributes ^= MethodAttributes.Abstract;
            attributes ^= MethodAttributes.Public;
            attributes |= (MethodAttributes.Virtual | MethodAttributes.Private | MethodAttributes.HideBySig | MethodAttributes.Final);
            var m = new MethodDefinition(name, attributes, interfaceMethod.ReturnType);
            m.Overrides.Add(interfaceMethod);
            m.Body = new MethodBody(m);
            var il = m.Body.GetILProcessor();
            il.Emit(OpCodes.Ldarg_0);
            foreach (var p in interfaceMethod.Parameters) {
                il.Emit(OpCodes.Ldarg, p);
            }
            il.Emit(OpCodes.Call, targetMethod);
            il.Emit(OpCodes.Ret);

            type.Methods.Add(m);

            ImplementedMethod = m;
        }
    }
}
