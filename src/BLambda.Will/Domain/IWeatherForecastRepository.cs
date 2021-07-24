using Ddd.Abstructions.Domain;

namespace BLambda.Will.Domain
{
    public interface IWeatherForecastRepository : IRepository<WeatherForecast>, IQueryAsync<WeatherForecast, string>
    {
    }
}
