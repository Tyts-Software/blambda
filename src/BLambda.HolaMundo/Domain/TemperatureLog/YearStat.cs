using Ddd.Abstructions.Domain;
using System.Collections.Generic;

namespace BLambda.HolaMundo.Domain.TemperatureLog
{
    public class YearStat : Entity, IAggregateRoot, IStat
    {
        public YearStat(string location, int year, Stat stat) =>
            (Location, Year, Stat) = (location, year, stat);

        public string Location { get; set; }
        public Stat Stat { get; set; }
        public int Year { get; set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Location;
            yield return Year;
        }
    }
}
