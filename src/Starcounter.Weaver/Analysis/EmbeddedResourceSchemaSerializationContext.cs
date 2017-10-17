
using Mono.Cecil;
using Starcounter.Hosting.Schema;
using Starcounter.Hosting.Schema.Serialization;

namespace Starcounter.Weaver.Analysis {

    public class EmbeddedResourceSchemaSerializationContext : SchemaSerializationContext {
        readonly string name;
        readonly ISchemaSerializer serializer;

        public const string DefaultResourceStreamName = "Starcounter.Schema.V2.2017";

        public EmbeddedResourceSchemaSerializationContext(
            ISchemaSerializer schemaSerializer,
            string resourceName,
            WeaverDiagnostics weaverDiagnostics) : base(weaverDiagnostics) {

            Guard.NotNull(schemaSerializer, nameof(schemaSerializer));
            Guard.NotNullOrEmpty(resourceName, nameof(resourceName));

            serializer = schemaSerializer;
            name = resourceName;
        }

        // Tests: null, read where name not exist, where serializer fail.
        public override DatabaseSchema Read(ModuleDefinition module) {
            Guard.NotNull(module, nameof(module));
            var embeddedSchemaData = module.ReadEmbeddedResource(name);
            return embeddedSchemaData != null ? serializer.Deserialize(embeddedSchemaData) : null;
        }

        // Test: nulls, serializer fail, module that has no resources, call twice (duplicate name)
        public override void Write(ModuleDefinition module, DatabaseSchema schema) {
            Guard.NotNull(module, nameof(module));
            Guard.NotNull(schema, nameof(schema));

            var data = serializer.Serialize(schema);
            var resource = new EmbeddedResource(name, ManifestResourceAttributes.Public, data);
            module.Resources.Add(resource);
        }
    }
}