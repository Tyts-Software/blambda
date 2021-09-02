using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace BLambda.Shall.Service.Weather
{
    public class WeatherService : IWeatherService
    {        
        private readonly HttpClient weatherApi;
        private readonly ILogger logger;

        public WeatherService(HttpClient httpClient, ILogger<WeatherService> logger)
        {
            this.weatherApi = httpClient;
            this.logger = logger;
        }

        public async Task<WeatherForecast[]> GetForecastAsync()
        {
            try
            {
                return await weatherApi.GetFromJsonAsync<WeatherForecast[]>("weather-forecast");
            }
            catch(Exception e)
            {
                logger.LogError($"{e.Message}", e);

                return Array.Empty<WeatherForecast>();
            }
        }
    }
}
