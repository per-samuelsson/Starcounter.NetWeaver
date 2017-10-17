
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Text;
using System;

namespace Starcounter.Hosting.Schema.Serialization {

    public class JsonNETSchemaSerializer : ISchemaSerializer {
        readonly IContractResolver contractResolver;

        public JsonNETSchemaSerializer() {
            contractResolver = null;
        }

        public JsonNETSchemaSerializer(IContractResolver resolver) {
            if (resolver == null) {
                throw new ArgumentNullException(nameof(resolver));
            }
            contractResolver = resolver;
        }
        
        DatabaseSchema ISchemaSerializer.Deserialize(byte[] schema) {
            if (schema == null) {
                throw new ArgumentNullException(nameof(schema));
            }

            var resolver = contractResolver ?? new DefaultContractResolver();
            var settings = new JsonSerializerSettings() { ContractResolver = resolver };

            var s = Encoding.UTF8.GetString(schema);
            return JsonConvert.DeserializeObject<DatabaseSchema>(s, settings);
        }

        byte[] ISchemaSerializer.Serialize(DatabaseSchema schema) {
            if (schema == null) {
                throw new ArgumentNullException(nameof(schema));
            }

            var resolver = contractResolver ?? new DefaultContractResolver();
            var settings = new JsonSerializerSettings() { ContractResolver = resolver };

            var s = JsonConvert.SerializeObject(schema, settings);
            return Encoding.UTF8.GetBytes(s ?? "");
        }
    }
}