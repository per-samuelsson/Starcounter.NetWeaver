
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Starcounter.Weaver.PropertyRewriting {
    /// <summary>
    /// Access method provider using a single given type and a pair of method
    /// sets, each static method mapped to a given data type supported.
    /// </summary>
    public class SingleTypeMethodSetProvider : ProviderOfAccessMethods {
        readonly Type type;
        readonly Dictionary<TypeReference, MethodReference> readMethods;
        readonly Dictionary<TypeReference, MethodReference> writeMethods;

        public SingleTypeMethodSetProvider(ModuleDefinition module, Type targetType, Dictionary<Type, MethodInfo> readers, Dictionary<Type, MethodInfo> writers) {
            Guard.NotNull(module, nameof(module));
            Guard.NotNull(targetType, nameof(targetType));
            Guard.NotNull(readers, nameof(readers));
            Guard.NotNull(writers, nameof(writers));

            type = targetType;
            readMethods = new Dictionary<TypeReference, MethodReference>(readers.Count);
            writeMethods = new Dictionary<TypeReference, MethodReference>(writers.Count);

            void BuildDictionary(ModuleDefinition m, Type t, Dictionary<Type, MethodInfo> from, Dictionary<TypeReference, MethodReference> to) {
                foreach (var item in from) {
                    if (item.Value == null) {
                        throw new ArgumentNullException($"Accessor for {item.Key} can't be null.");
                    }
                    if (item.Value.DeclaringType != t) {
                        throw new ArgumentException($"Accessor for {item.Key} ({item.Value.Name}) is not defined in {t}.");
                    }
                    to.Add(m.ImportReference(item.Key), m.ImportReference(item.Value));
                }
            }

            BuildDictionary(module, targetType, readers, readMethods);
            BuildDictionary(module, targetType, writers, writeMethods);
        }

        public override MethodReference ProvideReadMethod(TypeReference dataType) {
            MethodReference target;
            if (readMethods.TryGetValue(dataType, out target)) {
                return target;
            }

            // What else? Object - is that reference a database type?
            // Or do we have a transform registered, and can resolve it to that?
            // Enum? Enum is a built-in transform of all supported primititves.
            // TODO:
            throw new NotSupportedException($"Type {type} does not provide a read method for data type {dataType}");
        }

        public override MethodReference ProvideWriteMethod(TypeReference dataType) {
            MethodReference target;
            if (writeMethods.TryGetValue(dataType, out target)) {
                return target;
            }

            // What else? Object - is that reference a database type?
            // Or do we have a transform registered, and can resolve it to that?
            // Enum? Enum is a built-in transform of all supported primititves.
            // TODO:
            throw new NotSupportedException($"Type {type} does not provide a write method for data type {dataType}");
        }
    }
}
