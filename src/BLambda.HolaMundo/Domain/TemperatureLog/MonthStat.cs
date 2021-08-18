using Ddd.Abstructions.Domain;
using System.Collections.Generic;

namespace BLambda.HolaMundo.Domain.TemperatureLog
{
    public class MonthStat : Entity, IAggregateRoot, IStat
    {
        public MonthStat(string location, Month month, int year, Stat stat) =>
            (Location, Month, Year, Stat) = (location, month, year, stat);

        public string Location { get; set; }
        public Stat Stat { get; set; }
        public Month Month { get; set; }
        public int Year { get; set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Location;
            yield return Year;
            yield return Month;
        }
    }
}
