using BLambda.HolaMundo.Domain;
using Microsoft.Extensions.DependencyInjection;
using Tyts.Abstractions.Data;

namespace BLambda.HolaMundo.Data
{
    public static class StatupExtension
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            return services
                    .AddScoped<IRepositoryAsync<WeatherForecast>, WeatherForecastRepository>();
        }
    }
}
