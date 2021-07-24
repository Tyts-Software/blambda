using BLambda.Will.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using Ddd.Abstructions.Domain;
using System.Linq.Expressions;
using System.Threading;

namespace BLambda.Will.Data
{
    internal class WeatherForecastRepository : IWeatherForecastRepository
    {
        //Fake
        class FakeUnitOfWork : IUnitOfWork
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

            public IQueryable<WeatherForecast> Query<WeatherForecast>()
            {
                return (IQueryable<WeatherForecast>)store.AsQueryable();
            }

            public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException();
            }

            public Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException();
            }
        }

        public IUnitOfWork UnitOfWork => new FakeUnitOfWork();

        public Task<IEnumerable<WeatherForecast>> GetAllAsync()
        {
            return Task.FromResult(UnitOfWork.Query<WeatherForecast>().AsEnumerable());
        }

        public Task<WeatherForecast> GetSingleAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IQueryable<WeatherForecast>> FindAsync(Expression<Func<WeatherForecast, bool>> expression)
        {
            throw new NotImplementedException();
        }
    }
}
