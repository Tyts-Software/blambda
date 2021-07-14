using BLambda.HolaMundo.Domain;
using Tyts.Abstractions.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tyts.Abstractions.Data.Search;
using System;
using System.Collections.Concurrent;

namespace BLambda.HolaMundo.Data
{
    public class WeatherForecastRepository : IRepositoryAsync<WeatherForecast>
    {
        //Facked
        private static readonly ConcurrentBag<WeatherForecast> store = new ConcurrentBag<WeatherForecast>()
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


        public Task DeleteAsync(object id)
        {
            throw new System.NotImplementedException();
        }

        public Task<SearchResult<WeatherForecast>> FindAsync(ISearchParam param)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<WeatherForecast>> GetAllAsync()
        {
            return Task.FromResult(store.AsEnumerable());
        }

        public Task<WeatherForecast> GetSingleAsync(object id)
        {
            throw new System.NotImplementedException();
        }

        public Task SaveAsync(WeatherForecast item)
        {
            throw new System.NotImplementedException();
        }
    }
}
