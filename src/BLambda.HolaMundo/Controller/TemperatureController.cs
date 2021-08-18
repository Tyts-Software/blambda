using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DocumentModel;
using BLambda.HolaMundo.Domain;
using BLambda.HolaMundo.Domain.Seed;
using BLambda.HolaMundo.Domain.TemperatureLog;
using BLambda.HolaMundo.Helper;
using Ddd.DynamoDb;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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
        [ProducesResponseType(200, Type = typeof(Page<LocationStat>))]
        [ProducesResponseType(200, Type = typeof(IAsyncEnumerable<LocationStat>))]
        public async Task<ActionResult> GetAllCurrent([FromQuery] int? ps, [FromHeader(Name = Page.PAGINATION_TOKEN_HEADER)] string? pt)
        {
            if (ps.HasValue)
            {
                var page = await temperatureLog.GetAllPagedAsync<LocationStat>(new PageParam()
                {
                    PageSize = ps.Value,
                    PaginationToken = pt
                });

                logger.LogDebug($"{nameof(GetAllCurrent)} page => size:{page.Count()} last:{page.IsLast} pt:{page.PaginationToken}");

                Response.Headers.Add(Page.PAGINATION_TOKEN_HEADER, page.PaginationToken);
                return Ok(page);
            }

            logger.LogDebug($"{nameof(GetAllCurrent)}");
            return Ok(temperatureLog.GetAllAsync<LocationStat>());
        }        

        // GET api/temperature/vlc
        [HttpGet("{location}")]
        public async Task<LocationStat?> GetCurrent([UpperCase] string location)
        {
            return await temperatureLog.GetSingleOrDefaultAsync<LocationStat>(location, location);
        }

        // GET api/temperature/vlc/2021
        [HttpGet("{location}/{date:yyyy}")]
        public async Task<YearStat?> GetYearSummary([UpperCase] string location, string date)
        {
            return await temperatureLog.GetSingleOrDefaultAsync<YearStat>(location, date);
        }

        // GET api/temperature/vlc/2021-07
        [HttpGet("{location}/{date:yyyy-MM}")]
        public async Task<MonthStat?> GetMonthSummary([UpperCase] string location, string date)
        {
            return await temperatureLog.GetSingleOrDefaultAsync<MonthStat>(location, date);
        }
        
        // GET api/temperature/vlc/2021-07-30
        //[HttpGet("{location}/{day:regex(^\\d{{4}}-\\d{{2}}-\\d{{2}}$)}")]
        [HttpGet("{location}/{date:yyyy-MM-dd}")]
        public async Task<DayStat?> GetDaySummary([UpperCase] string location, string date)
        {
            var result = await temperatureLog.GetSingleOrDefaultAsync<DayStat>(location, date);
            return result;
        }

        // GET api/temperature/vlc/2021-07/*
        [HttpGet("{location}/{month:yyyy-MM}/*")]
        public IAsyncEnumerable<DayStat> GetForMonth([UpperCase] string location, string month)
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
        public IAsyncEnumerable<MonthStat> GetForYear([UpperCase] string location, string year)
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

            return HttpStatusCodeResult(304, "Not Modified");
        }

        private IActionResult HttpStatusCodeResult(int v1, string v2)
        {
            throw new NotImplementedException();
        }

        // POST api/temperature
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([ModelBinder(BinderType = typeof(StatModelBinder))] IStat stat)
        {
            if (stat == null)
            {
                return BadRequest();
            }

            await temperatureLog.SaveAsync(stat);

            return Ok();
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
        public async Task<IActionResult> Delete([UpperCase] string location, string? date)
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
