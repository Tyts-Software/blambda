using System;
using System.Collections.Generic;
using System.Linq;

namespace BLambda.HolaMundo.Domain.Seed
{
    public static class WeatherForecastSeed
    {
        [ThreadStatic]
        private static readonly IList<WeatherForecast> store = new List<WeatherForecast>()
        {
            new WeatherForecast
            {
                Date = new DateTime(2018, 05, 06),
                TemperatureC = 1,
                Summary = "Freezing"
            },
            new WeatherForecast
            {
                Date = new DateTime(2018, 05, 07),
                TemperatureC = 14,
                Summary = "Bracing"
            },
            new WeatherForecast
            {
                Date = new DateTime(2018, 05, 08),
                TemperatureC = -13,
                Summary = "Freezing"
            },
            new WeatherForecast
            {
                Date = new DateTime(2018, 05, 09),
                TemperatureC = -16,
                Summary = "Balmy"
            },
            new WeatherForecast
            {
                Date = new DateTime(2018, 05, 10),
                TemperatureC = -2,
                Summary = "Chilly"
            }
        };

        public static IQueryable<WeatherForecast> Query<WeatherForecast>()
        {
            return (IQueryable<WeatherForecast>)store.AsQueryable();
        }
    }
}
