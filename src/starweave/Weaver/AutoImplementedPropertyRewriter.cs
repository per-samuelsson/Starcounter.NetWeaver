
using Mono.Cecil;
using Mono.Cecil.Cil;
using Starcounter.Weaver;

namespace starweave.Weaver {
    
    public sealed class AutoImplementedPropertyRewriter {
        readonly DatabaseTypeState state;
        readonly Instruction[] setup;
        
        public AutoImplementedPropertyRewriter(DatabaseTypeState typeState) {
            Guard.NotNull(typeState, nameof(typeState));
            state = typeState;

            setup = new Instruction[4];
            setup[0] = Instruction.Create(OpCodes.Ldarg_0);
            setup[1] = Instruction.Create(OpCodes.Ldfld, typeState.DbId);
            setup[2] = Instruction.Create(OpCodes.Ldarg_0);
            setup[3] = Instruction.Create(OpCodes.Ldfld, typeState.DbRef);
        }
        
        public void Rewrite(AutoImplementedProperty property, MethodReference readMethod, MethodReference writeMethod) {
            Guard.NotNull(property, nameof(property));
            Guard.NotNull(readMethod, nameof(readMethod));
            Guard.NotNull(writeMethod, nameof(writeMethod));

            AssertValidReadMethod(property, readMethod);
            AssertValidWriteMethod(property, writeMethod);
            
            // For object writes, there might be a cast too.
            // TODO:

            var propDef = property.Property;
            AssertExpectedGetter(propDef.GetMethod);
            RewriteGetter(property, readMethod);

            var setter = propDef.SetMethod;
            if (setter != null) {
                AssertExpectedSetter(setter);
                RewriteSetter(property, writeMethod);
            }

            propDef.DeclaringType.Fields.Remove(property.BackingField);
        }

        void RewriteGetter(AutoImplementedProperty property, MethodReference readMethod) {
            // ldarg.0
            // ldfld [property.BackingField]
            // ret
            //
            // =>
            //
            // We want to replace that with
            // ldarg.0
            // ldfld dbid
            // ldarg.0
            // ldfld dbref
            // ldsfld crudHandle
            // call CRUD.Get[xxx]
            // ret
            //
            // (Nop's excluded) 

            var getter = property.Property.GetMethod;
            getter.Body.Instructions.Clear();
            var il = getter.Body.GetILProcessor();
            for (int i = 0; i < setup.Length; i++) {
                il.Append(setup[i]);
            }
            il.Append(il.Create(OpCodes.Ldsfld, state.GetPropertyHandle(property.Property)));
            il.Append(il.Create(OpCodes.Call, readMethod));
            il.Append(il.Create(OpCodes.Ret));
        }

        void RewriteSetter(AutoImplementedProperty property, MethodReference writeMethod) {
            // ldarg.0
            // ldarg.1
            // stfld [property.BackingField]
            // ret
            //
            // =>
            //
            // We want to replace that with
            // ldarg.0
            // ldfld dbid
            // ldarg.0
            // ldfld dbref
            // ldsfld crudHandle
            // ldarg.1
            // call CRUD.Set[xxx]
            // ret
            //
            // (Nop's excluded) 

            var getter = property.Property.SetMethod;
            getter.Body.Instructions.Clear();
            var il = getter.Body.GetILProcessor();
            for (int i = 0; i < setup.Length; i++) {
                il.Append(setup[i]);
            }
            il.Append(il.Create(OpCodes.Ldsfld, state.GetPropertyHandle(property.Property)));
            il.Append(il.Create(OpCodes.Ldarg_1));
            il.Append(il.Create(OpCodes.Call, writeMethod));
            il.Append(il.Create(OpCodes.Ret));
        }

        // This one will eventually be conditional, part of only
        // non-optimized builds. Initially, we'll be paranoid though.
        void AssertValidReadMethod(AutoImplementedProperty property, MethodReference method) {
            RewritingAssertionMethods.VerifyExpectedReadMethod(property, method);
        }

        // This one will eventually be conditional, part of only
        // non-optimized builds. Initially, we'll be paranoid though. 
        void AssertValidWriteMethod(AutoImplementedProperty property, MethodReference method) {
            RewritingAssertionMethods.VerifyExpectedWriteMethod(property, method);
        }

        // This one will eventually be conditional, part of only
        // non-optimized builds. Initially, we'll be paranoid though. 
        void AssertExpectedGetter(MethodDefinition getter) {
            RewritingAssertionMethods.VerifyExpectedOriginalGetter(getter);
        }

        // This one will eventually be conditional, part of only
        // non-optimized builds. Initially, we'll be paranoid though. 
        void AssertExpectedSetter(MethodDefinition setter) {
            RewritingAssertionMethods.VerifyExpectedOriginalSetter(setter);
        }
    }
}
