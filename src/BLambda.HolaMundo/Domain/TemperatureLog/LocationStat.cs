using Ddd.Abstructions.Domain;
using System;
using System.Collections.Generic;

namespace BLambda.HolaMundo.Domain.TemperatureLog
{
    public class LocationStat : Entity, IAggregateRoot, IStat
    {
        //[JsonConstructor]
        public LocationStat(string location, DateTime date, double cur) =>
            (Location, Date, Cur) = (location, date, cur);

        public string Location { get; set; }
        public Stat? Stat { get; set; }
        public double Cur { get; set; }

        private DateTime _date;        
        public DateTime Date
        {
            get => _date;
            set => _date = value.ToUniversalTime();
        }

        
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Location;
            yield return Date;
        }
    }
}
