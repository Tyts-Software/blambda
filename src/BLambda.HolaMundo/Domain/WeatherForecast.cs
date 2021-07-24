using Ddd.Abstructions.Domain;
using System;

namespace BLambda.HolaMundo.Domain
{
    public class WeatherForecast : Entity<string>, IAggregateRoot
    {
        public override string Id { get => Date.ToFileTimeUtc().ToString(); protected set { } }

        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public string Summary { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }
}
