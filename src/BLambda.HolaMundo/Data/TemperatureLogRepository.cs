using System;
using Ddd.DynamoDb;
using Microsoft.Extensions.Logging;
using Amazon.DynamoDBv2;
using BLambda.HolaMundo.Domain.TemperatureLog;

namespace BLambda.HolaMundo.Data
{
    internal class TemperatureLogRepository : DynamoDbDocumentRepository, ITemperatureLogRepository
    {        
        public TemperatureLogRepository(IAmazonDynamoDB client, TemperatureLogDbContext mapper, ILogger<TemperatureLogRepository> logger) 
            : base(client, mapper, Environment.GetEnvironmentVariable("TemperatureLogTableName") ?? "TemperatureLogTableName", logger)
        {



        }

        //public async Task<IEnumerable<WeatherForecast>> GetAllAsync() //string paginationToken = ""
        //{
        //    //return Task.FromResult(UnitOfWork.Query<WeatherForecast>().AsEnumerable());

        //    //string paginationToken = "";

        //    //// Get the Table ref from the Model
        //    //var table = _context.GetTargetTable<Reader>();

        //    //// If there's a PaginationToken
        //    //// Use it in the Scan options
        //    //// to fetch the next set
        //    //var scanOps = new ScanOperationConfig();
        //    //if (!string.IsNullOrEmpty(paginationToken))
        //    //{
        //    //    scanOps.PaginationToken = paginationToken;
        //    //}

        //    //// returns the set of Document objects
        //    //// for the supplied ScanOptions
        //    //var results = table.Scan(scanOps);
        //    //List<Document> data = await results.GetNextSetAsync();

        //    //// transform the generic Document objects
        //    //// into our Entity Model
        //    //IEnumerable<Reader> readers = _context.FromDocuments<Reader>(data);

        //    //// Pass the PaginationToken
        //    //// if available from the Results
        //    //// along with the Result set
        //    //return new ReaderViewModel
        //    //{
        //    //    PaginationToken = results.PaginationToken,
        //    //    Readers = readers,
        //    //    ResultsType = ResultsType.List
        //    //};

        //    /* The Non-Pagination approach */
        //    var scanConditions = new List<ScanCondition>() 
        //    { 
        //        new ScanCondition("Id", ScanOperator.IsNotNull) 
        //    };
        //    var searchResults = _context.ScanAsync<Reader>(scanConditions, null);
        //    return await searchResults.GetNextSetAsync();
        //}


        //public async Task<IQueryable<WeatherForecast>> FindAsync(Expression<Func<WeatherForecast, bool>> expression)
        //{
        //    var scanConditions = new List<ScanCondition>();
        //    if (!string.IsNullOrEmpty(searchReq.UserName))
        //        scanConditions.Add(new ScanCondition("Username", ScanOperator.Equal, searchReq.UserName));
        //    if (!string.IsNullOrEmpty(searchReq.EmailAddress))
        //        scanConditions.Add(new ScanCondition("EmailAddress", ScanOperator.Equal, searchReq.EmailAddress));
        //    if (!string.IsNullOrEmpty(searchReq.Name))
        //        scanConditions.Add(new ScanCondition("Name", ScanOperator.Equal, searchReq.Name));

        //    return await _context.ScanAsync<Reader>(scanConditions, null).GetRemainingAsync();
        //}
    }
}
