
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Starcounter.Weaver.Runtime {

    public class TypeSerializationAdvice {

        public Type Type { get; private set; }

        public IList<string> PrivateFields { get; private set; } = new List<string>();

        public IList<string> IgnoredProperties { get; private set; } = new List<string>();

        public TypeSerializationAdvice(Type type) {
            Type = type ?? throw new ArgumentNullException(nameof(type));
        }
        
        public List<MemberInfo> GetSerializableMembers(List<MemberInfo> defaultMembers) {
            const BindingFlags privateFieldBinding = BindingFlags.Instance | BindingFlags.NonPublic;

            foreach (var privateField in PrivateFields) {
                var f = Type.GetTypeInfo().GetField(privateField, privateFieldBinding);
                if (f == null) {
                    throw new Exception($"Internal error: Type {Type.FullName} declares no field named {privateField}");
                }
                defaultMembers.Add(f);
            }
            return defaultMembers;
        }

        public JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization, JsonProperty defaultProperty) {
            if (IgnoredProperties.Contains(member.Name)) {
                return null;
            }

            if (PrivateFields.Contains(member.Name)) {
                defaultProperty.Readable = true;
            }
            
            return defaultProperty;
        }
    }
}