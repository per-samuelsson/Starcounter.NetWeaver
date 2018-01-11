
namespace Starcounter.Hosting.Schema.Serialization {

    public interface ISchemaSerializer {

        byte[] Serialize(DatabaseSchema schema);

        DatabaseSchema Deserialize(byte[] schema);
    }
}