using Ddd.DynamoDb;

namespace BLambda.Will.Domain
{
    public interface IWeatherForecastRepository : IRepositoryAsync<WeatherForecast, string>
    {
    }
}
