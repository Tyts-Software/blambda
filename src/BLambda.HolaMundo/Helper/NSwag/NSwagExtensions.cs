#if GENERATE_API_CLIENT

using Microsoft.Extensions.DependencyInjection;

namespace BLambda.HolaMundo.Helper
{
    public static class NSwagExtensions
    {
        public static void AddNSwag(this IServiceCollection services)
        {
            // Register the Swagger services
            services.AddSwaggerDocument(d =>
            {
                //d.SchemaProcessors.Add(new MarkAsRequiredIfNonNullableSchemaProcessor());

                d.RequireParametersWithoutDefault = true;
                //d.AllowReferencesWithProperties = true;

                //prevent the required fields from accepting null as a value
                d.DefaultReferenceTypeNullHandling = NJsonSchema.Generation.ReferenceTypeNullHandling.NotNull;
            });
        }
    }
}

#endif