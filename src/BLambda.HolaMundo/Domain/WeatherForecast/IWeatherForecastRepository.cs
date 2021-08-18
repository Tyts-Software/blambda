using Ddd.DynamoDb;

namespace BLambda.HolaMundo.Domain
{
    public interface IWeatherForecastRepository : IRepositoryAsync<WeatherForecast, string>
    {
    }
}
