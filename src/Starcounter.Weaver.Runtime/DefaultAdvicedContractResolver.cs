
using System.Collections.Generic;

namespace Starcounter.Weaver.Runtime {

    public class DefaultAdvicedContractResolver : AdvicedContractResolver {

        public static IEnumerable<TypeSerializationAdvice> CreateDefaultAdvices() {
            var advices = new List<TypeSerializationAdvice>();

            var advice = new TypeSerializationAdvice(typeof(TypeSystem));
            advice.PrivateFields.Add("indexOfAllNamedTypes");
            advice.PrivateFields.Add("dataTypes");
            advice.IgnoredProperties.Add(nameof(TypeSystem.DataTypes));
            advice.IgnoredProperties.Add(nameof(TypeSystem.DatabaseTypes));
            advices.Add(advice);

            advice = new TypeSerializationAdvice(typeof(DatabaseSchema));
            advice.PrivateFields.Add("assemblies");
            advice.PrivateFields.Add("typeSystem");
            advice.IgnoredProperties.Add(nameof(DatabaseSchema.Assemblies));
            advice.IgnoredProperties.Add(nameof(DatabaseSchema.Types));
            advice.IgnoredProperties.Add(nameof(DatabaseSchema.TypeSystem));
            advices.Add(advice);

            advice = new TypeSerializationAdvice(typeof(DatabaseAssembly));
            advice.PrivateFields.Add("types");
            advice.IgnoredProperties.Add(nameof(DatabaseAssembly.DefiningSchema));
            advice.IgnoredProperties.Add(nameof(DatabaseAssembly.Types));
            advices.Add(advice);

            advice = new TypeSerializationAdvice(typeof(DatabaseType));
            advice.PrivateFields.Add("nameHandle");
            advice.PrivateFields.Add("baseNameHandle");
            advice.PrivateFields.Add("properties");
            advice.IgnoredProperties.Add(nameof(DatabaseType.DefiningAssembly));
            advice.IgnoredProperties.Add(nameof(DatabaseType.Properties));
            advice.IgnoredProperties.Add(nameof(DatabaseType.BaseTypeName));
            advice.IgnoredProperties.Add(nameof(DatabaseType.FullName));
            advices.Add(advice);

            advice = new TypeSerializationAdvice(typeof(DatabaseProperty));
            advice.PrivateFields.Add("name");
            advice.PrivateFields.Add("dataType");
            advice.IgnoredProperties.Add(nameof(DatabaseProperty.DeclaringType));
            advice.IgnoredProperties.Add(nameof(DatabaseProperty.Name));
            advice.IgnoredProperties.Add(nameof(DatabaseProperty.DataType));
            advices.Add(advice);

            return advices;
        }

        public DefaultAdvicedContractResolver() : base(CreateDefaultAdvices()) {

        }
    }
}