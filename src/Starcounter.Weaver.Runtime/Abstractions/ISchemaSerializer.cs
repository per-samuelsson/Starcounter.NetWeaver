
namespace Starcounter.Weaver.Runtime.Abstractions {

    public interface ISchemaSerializer {

        byte[] Serialize(DatabaseSchema schema);

        DatabaseSchema Deserialize(byte[] schema);
    }
}