using System.Threading.Tasks;

namespace BLambda.Shall.Service.Weather
{
    internal interface IWeatherService
    {
        Task<WeatherForecast[]> GetForecastAsync();
    }
}
