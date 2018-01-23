
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

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
            foreach (var interfaceParameter in interfaceMethod.Parameters) {
                m.Parameters.Add(new ParameterDefinition(interfaceParameter.Name, interfaceParameter.Attributes, interfaceParameter.ParameterType));
            }
            m.Overrides.Add(interfaceMethod);

            m.Body = new MethodBody(m);
            var il = m.Body.GetILProcessor();
            il.Append(il.Create(OpCodes.Ldarg_0));
            foreach (var p in m.Parameters) {
                il.Append(il.Create(OpCodes.Ldarg, p));
            }
            il.Append(il.Create(OpCodes.Call, targetMethod));
            il.Append(il.Create(OpCodes.Ret));

            // Make sure we optimize parameter stack loading on what
            // we generate
            // https://stackoverflow.com/questions/10231409/creating-an-il-instruction-with-an-inline-argument-using-mono-cecil
            il.Body.OptimizeMacros();

            type.Methods.Add(m);

            ImplementedMethod = m;
        }
    }
}
