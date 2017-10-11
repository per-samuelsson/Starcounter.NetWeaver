
using Newtonsoft.Json;
using System.Text;

namespace Starcounter.Hosting.Schema {

    public class JsonNETSchemaSerializer : ISchemaSerializer {

        DatabaseSchema ISchemaSerializer.Deserialize(byte[] schema) {
            var s = Encoding.UTF8.GetString(schema);
            return JsonConvert.DeserializeObject(s) as DatabaseSchema;
        }

        byte[] ISchemaSerializer.Serialize(DatabaseSchema schema) {
            var s = JsonConvert.SerializeObject(schema);
            return Encoding.UTF8.GetBytes(s ?? "");
        }
    }
}