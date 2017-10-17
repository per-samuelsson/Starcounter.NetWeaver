
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Starcounter.Hosting.Schema.Serialization {
    
    public class AdvicedContractResolver : DefaultContractResolver {
        readonly Dictionary<Type, TypeSerializationAdvice> advices;
        
        public AdvicedContractResolver(IEnumerable<TypeSerializationAdvice> typeAdvices) {
            advices = new Dictionary<Type, TypeSerializationAdvice>(typeAdvices.Count());
            foreach (var advice in typeAdvices) {
                advices.Add(advice.Type, advice);
            }
        }

        protected override List<MemberInfo> GetSerializableMembers(Type objectType) {
            var defaultMembers = base.GetSerializableMembers(objectType);
            var custom = GetAdvice(objectType);
            return custom != null ? custom.GetSerializableMembers(defaultMembers) : defaultMembers;
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization) {
            var p = base.CreateProperty(member, memberSerialization);
            var custom = GetAdvice(member.DeclaringType);
            return custom != null ? custom.CreateProperty(member, memberSerialization, p) : p;
        }

        TypeSerializationAdvice GetAdvice(Type type) {
            TypeSerializationAdvice advice;
            return advices.TryGetValue(type, out advice) ? advice : null;
        }
    }
}
