using Ddd.Abstructions.Domain;

namespace BLambda.HolaMundo.Domain
{
    public interface IWeatherForecastRepository : IRepository<WeatherForecast>, IQueryAsync<WeatherForecast, string>
    {
    }
}
