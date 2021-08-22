using System.Collections.Generic;
using BLambda.Will.Domain;
using Microsoft.AspNetCore.Mvc;

namespace BLambda.Will.Controllers
{
    [Route("api/[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private IWeatherForecastRepository repository;

        public WeatherForecastController(IWeatherForecastRepository repository)
        {
            this.repository = repository;
        }

        // GET api/weather-forecast
        [HttpGet]
        public IAsyncEnumerable<WeatherForecast> Get()
        {
            return repository.GetAllAsync();
        }

        // GET api/weather-forecast/5
        [HttpGet("{id}")]
        public WeatherForecast Get(int id)
        {
            throw new System.NotImplementedException();
        }

        // POST api/weather-forecast
        [HttpPost]
        public void Post([FromBody] WeatherForecast value)
        {
        }

        // PUT api/weather-forecast/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] WeatherForecast value)
        {
        }

        // DELETE api/weather-forecast/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
