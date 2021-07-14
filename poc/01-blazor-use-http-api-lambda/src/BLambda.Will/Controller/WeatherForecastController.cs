using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BLambda.Will.Domain;
using Microsoft.AspNetCore.Mvc;
using Tyts.Abstractions.Data;

namespace BLambda.Will.Controllers
{
    [Route("api/[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        //FackeRepository
        private IRepositoryAsync<WeatherForecast> repository;

        public WeatherForecastController(IRepositoryAsync<WeatherForecast> repository)
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
