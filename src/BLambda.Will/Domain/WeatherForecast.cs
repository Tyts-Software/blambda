using System;
using Tyts.Abstractions.Data;

namespace BLambda.Will.Domain
{
    public class WeatherForecast: IEntity
    {
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public string Summary { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }
}
