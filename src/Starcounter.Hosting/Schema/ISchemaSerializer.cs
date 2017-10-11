using System.IO;

namespace Starcounter.Hosting.Schema {
    interface ISchemaSerializer {
        Stream Serialize(DatabaseSchema schema);
        DatabaseSchema Deserialize(Stream stream);
    }
}
