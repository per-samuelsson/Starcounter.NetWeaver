
namespace Starcounter.Hosting.Schema {
    public static class SchemaSerialization {
        public const string EmbeddedResourceStreamName = "Starcounter.Schema.V2.2017";

        public static ISchemaSerializer CreateDefaultSerializer() {
            return new JsonNETSchemaSerializer();
        }
    }
}