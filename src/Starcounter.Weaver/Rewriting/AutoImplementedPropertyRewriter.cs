
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Starcounter.Weaver.Rewriting {
    // What does the rewriter need?
    // The type with those fields.
    // Then the property, and getter and setter with possible cast.

    public sealed class AutoImplementedPropertyRewriter {
        readonly TypeDefinition type;
        readonly Instruction[] setup;

        public AutoImplementedPropertyRewriter(TypeDefinition typeDefinition) {
            Guard.NotNull(typeDefinition, nameof(typeDefinition));
            type = typeDefinition;

            setup = new Instruction[4];
            setup[0] = Instruction.Create(OpCodes.Ldarg_0);
            setup[1] = Instruction.Create(OpCodes.Ldfld, typeDefinition.GetIdField());
            setup[2] = Instruction.Create(OpCodes.Ldarg_0);
            setup[3] = Instruction.Create(OpCodes.Ldfld, typeDefinition.GetRefField());

            // Pre-create instruction sequence with fields, like
            // ldarg.0
            // ldfld dbid
            // ldarg.0
            // ldfld dbref
            // ldsfld crudHandle
            //
            // When writing tests, check if this work even if those fields are in a parent
            // class (we emit them only on root level).
            // TODO:
        }
        
        public void Rewrite(AutoImplementedProperty property, MethodDefinition getMethod, MethodDefinition setMethod) {
            Guard.NotNull(property, nameof(property));
            Guard.NotNull(getMethod, nameof(getMethod));
            Guard.NotNull(setMethod, nameof(setMethod));

            // For primitives, we could verify property type is same as target method
            // return type.
            //
            // For object writes, there might be a cast too.

            var propDef = property.Property;
            VerifyExpectedGetter(propDef.GetMethod);
            RewriteGetter(property, getMethod);

            var setter = propDef.SetMethod;
            if (setter != null) {
                VerifyExpectedSetter(setter);
                RewriteSetter(property, setMethod);
            }
        }

        void RewriteGetter(AutoImplementedProperty property, MethodDefinition getMethod) {
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
            il.Append(il.Create(OpCodes.Ldsfld, property.Property.GetPropertyHandleField()));
            il.Append(il.Create(OpCodes.Call, getMethod));
            il.Append(il.Create(OpCodes.Ret));
        }

        void RewriteSetter(AutoImplementedProperty property, MethodDefinition setMethod) {
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

            var getter = property.Property.GetMethod;
            getter.Body.Instructions.Clear();
            var il = getter.Body.GetILProcessor();
            for (int i = 0; i < setup.Length; i++) {
                il.Append(setup[i]);
            }
            il.Append(il.Create(OpCodes.Ldsfld, property.Property.GetPropertyHandleField()));
            il.Append(il.Create(OpCodes.Ldarg_1));
            il.Append(il.Create(OpCodes.Call, setMethod));
            il.Append(il.Create(OpCodes.Ret));
        }

        // This one will eventually be conditional, part of only
        // non-optimized builds. Initially, we'll be paranoid though. 
        void VerifyExpectedGetter(MethodDefinition getter) {
            RewritingAssertionMethods.VerifyExpectedOriginalGetter(getter);
        }

        // This one will eventually be conditional, part of only
        // non-optimized builds. Initially, we'll be paranoid though. 
        void VerifyExpectedSetter(MethodDefinition setter) {
            RewritingAssertionMethods.VerifyExpectedOriginalSetter(setter);
        }
    }
}
