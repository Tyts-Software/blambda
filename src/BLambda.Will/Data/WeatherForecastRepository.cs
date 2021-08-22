using BLambda.Will.Domain;
using System;
using Ddd.DynamoDb;
using Amazon.DynamoDBv2.DataModel;

namespace BLambda.Will.Data
{
    internal class WeatherForecastRepository : DynamoDbRepository<WeatherForecast, string>, IWeatherForecastRepository
    {
        public WeatherForecastRepository(IDynamoDBContext context)
            : base(context, Environment.GetEnvironmentVariable("WeatherForecastTableName") ?? "WeatherForecastTableName")
        {
        }
    }
}
