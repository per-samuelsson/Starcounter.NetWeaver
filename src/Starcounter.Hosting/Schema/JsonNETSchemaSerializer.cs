
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using System;

namespace Starcounter.Hosting.Schema {

    public class JsonNETSchemaSerializer : ISchemaSerializer {
        DefaultContractResolver contractResolver;

        class FilteredContractResolver : DefaultContractResolver {
            readonly IList<string> filter;
            
            public FilteredContractResolver(IList<string> propertiesToIgnore) {
                filter = propertiesToIgnore;
            }

            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization) {
                var qualifiedName = member.DeclaringType.Name + member.Name;
                if (filter.Contains(qualifiedName)) {
                    return null;
                }
                return base.CreateProperty(member, memberSerialization);
            }
        }

        public JsonNETSchemaSerializer() {
            contractResolver = new FilteredContractResolver(
                new[] {
                    typeof(DatabaseAssembly).Name + nameof(DatabaseAssembly.DefiningSchema),
                    typeof(DatabaseType).Name + nameof(DatabaseType.DefiningAssembly),
                    typeof(DatabaseProperty).Name + nameof(DatabaseProperty.DeclaringType)
                    }
                );
        }
        
        DatabaseSchema ISchemaSerializer.Deserialize(byte[] schema) {
            if (schema == null) {
                throw new ArgumentNullException(nameof(schema));
            }

            var s = Encoding.UTF8.GetString(schema);
            return JsonConvert.DeserializeObject<DatabaseSchema>(s);
        }

        byte[] ISchemaSerializer.Serialize(DatabaseSchema schema) {
            if (schema == null) {
                throw new ArgumentNullException(nameof(schema));
            }
            
            var s = JsonConvert.SerializeObject(schema, new JsonSerializerSettings() { ContractResolver = contractResolver });
            return Encoding.UTF8.GetBytes(s ?? "");
        }
    }
}