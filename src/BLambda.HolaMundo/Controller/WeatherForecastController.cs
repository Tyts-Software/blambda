using System.Collections.Generic;
using System.Threading.Tasks;
using BLambda.HolaMundo.Domain;
using Microsoft.AspNetCore.Mvc;

namespace BLambda.HolaMundo.Controllers
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
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            return await repository.GetAllAsync();
        }

        // GET api/weather-forecast/5
        [HttpGet("{id}")]
        public async Task<WeatherForecast> Get(string id)
        {
            return await repository.GetSingleAsync(id);
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
