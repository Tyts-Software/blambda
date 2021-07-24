using BLambda.Will.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace BLambda.Will.Data
{
    public static class StatupExtension
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            return services
                    .AddScoped<IWeatherForecastRepository, WeatherForecastRepository>();
        }
    }
}
