using BLambda.HolaMundo.Domain;
using BLambda.HolaMundo.Domain.TemperatureLog;
using Microsoft.Extensions.DependencyInjection;

namespace BLambda.HolaMundo.Data
{
    public static class StatupExtension
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            return services
                    .AddSingleton<TemperatureLogDbContext>()
                    .AddScoped<IWeatherForecastRepository, WeatherForecastRepository>()
                    .AddScoped<ITemperatureLogRepository, TemperatureLogRepository>();
                    //.AddScoped<ITemperatureLogRepository<DayStat>, TemperatureLogRepository<DayStat>>()
                    //.AddScoped<ITemperatureLogRepository<MonthStat>, TemperatureLogRepository<MonthStat>>()
                    //.AddScoped<ITemperatureLogRepository<YearStat>, TemperatureLogRepository<YearStat>>()
                    //.AddScoped<ITemperatureLogRepository<LocationStat>, TemperatureLogRepository<LocationStat>>();
        }
    }
}
