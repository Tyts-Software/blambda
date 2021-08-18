using Amazon.DynamoDBv2.DataModel;
using Ddd.Abstructions.Domain;
using Ddd.DynamoDb;
using System;
using System.Collections.Generic;

namespace BLambda.HolaMundo.Domain
{
    [DynamoDBTable("WeatherForecast")]
    public class WeatherForecast : DynamoDbEntity<string>, IAggregateRoot
    {
        [DynamoDBHashKey]
        public string Location { get; set; }
        [DynamoDBRangeKey(AttributeName = "Date")]
        public DateTime Date { get; set; }

        [DynamoDBProperty(AttributeName = "Temperature")]
        public int TemperatureC { get; set; }

        public string Summary { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Location;
            yield return Date;
        }
    }
}
