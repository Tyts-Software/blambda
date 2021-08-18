using Ddd.Abstructions.Domain;
using System;
using System.Collections.Generic;
using static BLambda.HolaMundo.Domain.Helper.DateTimeExtensions;

namespace BLambda.HolaMundo.Domain.TemperatureLog
{
    public class DayStat : Entity, IAggregateRoot, IStat
    {
        public DayStat(string location, DateTime date, Stat stat) =>
            (Location, Date, Stat) = (location, date, stat);

        public string Location { get; set; }
        public Stat Stat { get; set; }

        private DateTime _date;
        public DateTime Date 
        {
            get => _date;
            set => _date = value.Truncate(DateTimeComponents.Day).ToUniversalTime();
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Location;
            yield return Date;
        }
    }
}
