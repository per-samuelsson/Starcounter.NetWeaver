
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
            var result = JsonConvert.DeserializeObject<DatabaseSchema>(s, settings);

            return MaterializeAfterDeserialization(result);
        }

        DatabaseSchema MaterializeAfterDeserialization(DatabaseSchema schema) {
            foreach (var assembly in schema.Assemblies) {
                assembly.DefiningSchema = schema;
                foreach (var type in assembly.Types) {
                    type.DefiningAssembly = assembly;
                    foreach (var property in type.Properties) {
                        property.DeclaringType = type;
                    }
                }
            }
            return schema;
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