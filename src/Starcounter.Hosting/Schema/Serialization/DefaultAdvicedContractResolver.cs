
using System.Collections.Generic;

namespace Starcounter.Hosting.Schema.Serialization {

    public class DefaultAdvicedContractResolver : AdvicedContractResolver {

        public static IEnumerable<TypeSerializationAdvice> CreateDefaultAdvices() {
            var advices = new List<TypeSerializationAdvice>();

            var advice = new TypeSerializationAdvice(typeof(DatabaseSchema));
            advice.PrivateFields.Add("assemblies");
            advice.IgnoredProperties.Add(nameof(DatabaseSchema.Assemblies));
            advices.Add(advice);

            advice = new TypeSerializationAdvice(typeof(DatabaseAssembly));
            advice.PrivateFields.Add("types");
            advice.IgnoredProperties.Add(nameof(DatabaseAssembly.DefiningSchema));
            advice.IgnoredProperties.Add(nameof(DatabaseAssembly.Types));
            advices.Add(advice);

            advice = new TypeSerializationAdvice(typeof(DatabaseType));
            advice.PrivateFields.Add("properties");
            advice.IgnoredProperties.Add(nameof(DatabaseType.DefiningAssembly));
            advice.IgnoredProperties.Add(nameof(DatabaseType.Properties));
            advices.Add(advice);

            advice = new TypeSerializationAdvice(typeof(DatabaseProperty));
            advice.IgnoredProperties.Add(nameof(DatabaseProperty.DeclaringType));
            advices.Add(advice);

            return advices;
        }

        public DefaultAdvicedContractResolver() : base(CreateDefaultAdvices()) {

        }
    }
}