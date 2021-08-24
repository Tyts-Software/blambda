using NJsonSchema;
using NJsonSchema.Generation;

namespace BLambda.HolaMundo.Helper
{
    public class MarkAsRequiredIfNonNullableSchemaProcessor : ISchemaProcessor
    {
        public void Process(SchemaProcessorContext context)
        {
            foreach (var (propName, prop) in context.Schema.Properties)
            {
                if (!prop.IsNullable(SchemaType.OpenApi3))
                {
                    prop.IsRequired = true;
                }
            }
        }
    }
}
