using BLambda.HolaMundo.Domain.TemperatureLog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BLambda.HolaMundo.Domain.Seed
{
    public static class TemperatureLogSeed
    {
        private static readonly IList<IStat> store = new List<IStat>()
        {
            new LocationStat(
                location: "VLC", 
                date: new DateTime(2021, 7, 31, 15, 20, 30, DateTimeKind.Utc), 
                cur: 23.4
            ){
                Stat = new(Min: 15.7, Max: 34.4, Avg: 23.5)
            },

            new DayStat(
                location: "VLC",  
                date: new DateTime(2021, 7, 30, 0, 0, 0, DateTimeKind.Utc), 
                stat: new(Min: 15.7, Max: 34.4, Avg: 24.0)
            ),
            new DayStat(
                location: "VLC",
                date: new DateTime(2021, 7, 29, 0, 0, 0, DateTimeKind.Utc),
                stat: new(Min: 15.8, Max: 33.3, Avg: 23.0)
            ),

            new MonthStat(
                location: "VLC", 
                month: Month.July, 
                year: 2021, 
                stat: new(Min: 15.7, Max: 34.4, Avg: 23.5)
            ),

            new YearStat(
                location: "VLC",
                year: 2021,
                stat: new(Min: 15.7, Max: 34.4, Avg: 23.5)
            ),

            //KBP
            new LocationStat(
                location: "KBP",
                date: new DateTime(2021, 7, 31, 15, 22, 30, DateTimeKind.Utc),
                cur: 34.3
            ){
                Stat = new(Min: 12.4, Max: 34.3, Avg: 22.5)
            },

            new DayStat(
                location: "KBP",
                date: new DateTime(2021, 7, 30, 0, 0, 0, DateTimeKind.Utc),
                stat: new(Min: 12.4, Max: 32.1, Avg: 22.0)
            ),
            new DayStat(
                location: "KBP",
                date: new DateTime(2021, 7, 29, 0, 0, 0, DateTimeKind.Utc),
                stat: new(Min: 15.5, Max: 33.1, Avg: 21.0)
            ),

            new MonthStat(
                location: "KBP",
                month: Month.July,
                year: 2021,
                stat: new(Min: 12.4, Max: 34.3, Avg: 22.5)
            ),

            new YearStat(
                location: "KBP",
                year: 2021,
                stat: new(Min: 12.4, Max: 34.3, Avg: 22.5)
            )
        };

        public static IQueryable<T> Query<T>() where T: IStat
        {
            return store.AsQueryable().OfType<T>();
        }
    }
}
