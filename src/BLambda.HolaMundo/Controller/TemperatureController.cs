using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Mime;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DocumentModel;
using BLambda.HolaMundo.Domain.Seed;
using BLambda.HolaMundo.Domain.TemperatureLog;
using BLambda.HolaMundo.Helper;
using Ddd.DynamoDb;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

#if GENERATE_API_CLIENT
using NJsonSchema.Annotations;
using NSwag.Annotations;
#endif

namespace BLambda.HolaMundo.Controllers
{
    //[ApiController]
    [Route("api/[controller]")]
    public class TemperatureController : ControllerBase
    {
        private readonly ILogger<TemperatureController> logger;
        private readonly ITemperatureLogRepository temperatureLog;

        public TemperatureController(ITemperatureLogRepository temperatureLog, ILogger<TemperatureController> logger)
        {
            this.logger = logger;
            this.temperatureLog = temperatureLog;
        }

        // GET api/temperature?ps=2 & Headers["X-Pagination-Token"]
        [HttpGet]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<LocationStat>))]
        //[ProducesResponseType(200, Type = typeof(IAsyncEnumerable<LocationStat>))]
        public async Task<ActionResult> GetAllCurrent([FromQuery(Name = "ps")] int? pSize = default, [FromHeader(Name = Page.PAGINATION_TOKEN_HEADER)] string? pToken = default)
        {
            if (pSize.HasValue)
            {
                var page = await temperatureLog.GetAllPagedAsync<LocationStat>(new PageParam()
                {
                    PageSize = pSize.Value,
                    PaginationToken = pToken
                });
                
                Response.Headers.Add(Page.PAGINATION_TOKEN_HEADER, page.PaginationToken);
                Debug.WriteLine($"{nameof(GetAllCurrent)} page => size:{page.Size} last:{page.IsLast} pt:{page.PaginationToken}");
                return Ok(page);
            }

            Debug.WriteLine($"{nameof(GetAllCurrent)}");
            return Ok(temperatureLog.GetAllAsync<LocationStat>());
        }        

        // GET api/temperature/vlc
        [HttpGet("{location}")]
        public async Task<LocationStat?> GetCurrent([FromRoute][UpperCase][NotNull] string location)
        {
            return await temperatureLog.GetSingleOrDefaultAsync<LocationStat>(location, nameof(LocationStat));
        }

        // GET api/temperature/vlc/2021
        [OpenApiIgnore] 
        [HttpGet("{location}/{date:yyyy}")]
        public async Task<YearStat?> GetYearSummary([UpperCase] string location, string date)
        {
            return await temperatureLog.GetSingleOrDefaultAsync<YearStat>(location, date);
        }

        // GET api/temperature/vlc/2021-07
        [OpenApiIgnore]
        [HttpGet("{location}/{date:yyyy-MM}")]
        public async Task<MonthStat?> GetMonthSummary([UpperCase] string location, string date)
        {
            return await temperatureLog.GetSingleOrDefaultAsync<MonthStat>(location, date);
        }

        // GET api/temperature/vlc/2021-07-30
        //[HttpGet("{location}/{day:regex(^\\d{{4}}-\\d{{2}}-\\d{{2}}$)}")]
        [HttpGet("{location}/{date:yyyy-MM-dd}")]
        public async Task<DayStat?> GetDaySummary([FromRoute][UpperCase][NotNull] string location, [FromRoute][NotNull] string date)
        {
            var result = await temperatureLog.GetSingleOrDefaultAsync<DayStat>(location, date);
            return result;
        }

        /// <summary>
        /// Get daily stast for the month        
        /// </summary>
        /// <param name="location">An IATA airport code, also known as an IATA location identifier, IATA station code, or simply a location identifier, is a three-letter geocode designating many airports and metropolitan areas around the world, defined by the International Air Transport Association (IATA).</param>
        /// <param name="month">Month in format: yyyy-MM</param>
        /// <returns></returns>
        /// <example>
        /// Ex.: GET api/temperature/vlc/2021-07/*
        /// </example>
        [HttpGet("{location}/{month:yyyy-MM}/*")]
        public IAsyncEnumerable<DayStat> GetForMonthAsync([FromRoute][UpperCase][NotNull] string location, [FromRoute][NotNull] string month)
        {
            var filter = new QueryFilter("PK", QueryOperator.Equal, location);
            filter.AddCondition("SK", QueryOperator.BeginsWith, $"{nameof(DayStat)}#{month}-");
            var query = new QueryOperationConfig
            {
                Limit = 31,
                Filter = filter
            };

            //var stats = temperatureLog.QueryAsync<DayStat>(qConfig);
            //await foreach (var stat in stats)
            //{
            //    yield return stat;
            //}

            return temperatureLog.QueryAsync<DayStat>(query);
        }

        // GET api/temperature/vlc/2021/*
        [HttpGet("{location}/{year:yyyy}/*")]
        public IAsyncEnumerable<MonthStat> GetForYear([FromRoute][UpperCase][NotNull] string location, [FromRoute][NotNull] string year)
        {
            var filter = new QueryFilter("PK", QueryOperator.Equal, location);
            filter.AddCondition("SK", QueryOperator.BeginsWith, $"{nameof(MonthStat)}#{year}-");
            var query = new QueryOperationConfig
            {
                Limit = 12,
                Filter = filter
            };

            return temperatureLog.QueryAsync<MonthStat>(query);
        }


        // POST api/temperature/seed
        [HttpPost("seed")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status304NotModified)]
        public async Task<IActionResult> Seed()
        {
            var isEmpty = await temperatureLog.IsEmpty();
            if (isEmpty)
            {
                await temperatureLog.SaveAsync(TemperatureLogSeed.Query<IStat>());
                
                return CreatedAtAction(nameof(Seed), "Table is seeded");
            }

            return new StatusCodeResult(304);
        }

        // POST api/temperature
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status304NotModified)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromBody][ModelBinder(BinderType = typeof(StatModelBinder))][NotNull] IStat stat)
        {
            if (stat == null)
            {
                return BadRequest();
            }

            return await temperatureLog.SaveAsync(stat)
                ? Ok()
                : new StatusCodeResult(304);
        }

        //// PUT api/temperature/vlc
        //[HttpPut("{location}/{date?}")]
        //public void Put([UpperCase] string location, string? date, [FromBody] [ModelBinder(BinderType = typeof(StatModelBinder))] IStat stat)
        //{
        //}

        // DELETE api/temperature/vlc
        [HttpDelete("{location}/{date?}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromRoute][UpperCase][NotNull] string location, string? date)
        {
            if (location == null && date == null)
            {
                return BadRequest();
            }

            return await temperatureLog.DeleteAsync(location, date ?? nameof(LocationStat))
                ? Ok()
                : NotFound();
        }
    }
}
