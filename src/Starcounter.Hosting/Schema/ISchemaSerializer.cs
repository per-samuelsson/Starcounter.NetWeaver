
namespace Starcounter.Hosting.Schema {
    public interface ISchemaSerializer {
        byte[] Serialize(DatabaseSchema schema);
        DatabaseSchema Deserialize(byte[] schema);
    }
}