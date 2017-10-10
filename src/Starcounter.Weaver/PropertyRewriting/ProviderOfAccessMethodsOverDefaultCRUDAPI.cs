
using Mono.Cecil;
using Starcounter2.Internal.WeaverFacade;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Starcounter.Weaver.PropertyRewriting {
    public class ProviderOfAccessMethodsOverDefaultCRUDAPI {

        public static ProviderOfAccessMethods Create(ModuleDefinition module) {
            var readers = new Dictionary<Type, MethodInfo>();
            var writers = new Dictionary<Type, MethodInfo>();

            var t = typeof(CRUD);
            var bindingFlags = BindingFlags.Static;
            readers.Add(typeof(int), t.GetMethod(nameof(CRUD.GetInt), bindingFlags));
            writers.Add(typeof(int), t.GetMethod(nameof(CRUD.SetInt), bindingFlags));
            readers.Add(typeof(int?), t.GetMethod(nameof(CRUD.GetNullableInt), bindingFlags));
            writers.Add(typeof(int?), t.GetMethod(nameof(CRUD.SetNullableInt), bindingFlags));

            return new SingleTypeMethodSetProvider(module, t, readers, writers);
        }
    }
}
