
using Starcounter.Hosting.Schema;
using Starcounter.Hosting.Schema.Serialization;
using System;

namespace Starcounter.Weaver.Tests {

    internal class DelegatingSchemaSerializer : ISchemaSerializer {
        readonly Func<DatabaseSchema, byte[]> serializer;
        readonly Func<byte[], DatabaseSchema> deserializer;

        public DelegatingSchemaSerializer(Func<DatabaseSchema, byte[]> s = null, Func<byte[], DatabaseSchema> d = null) {
            serializer = s;
            deserializer = d;
        }
        
        DatabaseSchema ISchemaSerializer.Deserialize(byte[] schema) {
            return deserializer?.Invoke(schema);
        }

        byte[] ISchemaSerializer.Serialize(DatabaseSchema schema) {
            return serializer?.Invoke(schema);
        }
    }
}