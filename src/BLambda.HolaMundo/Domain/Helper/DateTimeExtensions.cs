using System;

namespace BLambda.HolaMundo.Domain.Helper
{
    public static class DateTimeExtensions
    {
        public enum DateTimeComponents
        {
            Second, Minute, Hour, Day
        }

        public static DateTime Round(this DateTime d, DateTimeComponents rt)
        {
            return rt switch
            {
                DateTimeComponents.Second => (d.Millisecond >= 500)
                        ? new DateTime(d.Year, d.Month, d.Day, d.Hour, d.Minute, d.Second, d.Kind).AddSeconds(1)
                        : new DateTime(d.Year, d.Month, d.Day, d.Hour, d.Minute, d.Second, d.Kind),
                DateTimeComponents.Minute => (d.Second >= 30)
                        ? new DateTime(d.Year, d.Month, d.Day, d.Hour, d.Minute, 0, d.Kind).AddMinutes(1)
                        : new DateTime(d.Year, d.Month, d.Day, d.Hour, d.Minute, 0, d.Kind),
                DateTimeComponents.Hour => (d.Minute >= 30)
                        ? new DateTime(d.Year, d.Month, d.Day, d.Hour, 0, 0, d.Kind).AddHours(1)
                        : new DateTime(d.Year, d.Month, d.Day, d.Hour, 0, 0, d.Kind),
                DateTimeComponents.Day => (d.Hour >= 12)
                        ? new DateTime(d.Year, d.Month, d.Day, 0, 0, 0, d.Kind).AddDays(1)
                        : new DateTime(d.Year, d.Month, d.Day, 0, 0, 0, d.Kind),
                _ => throw new InvalidOperationException(),
            };
        }

        public static DateTime Truncate(this DateTime d, DateTimeComponents rt) 
        {
            return rt switch
            {
                DateTimeComponents.Second => new DateTime(d.Year, d.Month, d.Day, d.Hour, d.Minute, d.Second, d.Kind),
                DateTimeComponents.Minute => new DateTime(d.Year, d.Month, d.Day, d.Hour, d.Minute, 0, d.Kind),
                DateTimeComponents.Hour => new DateTime(d.Year, d.Month, d.Day, d.Hour, 0, 0, d.Kind),
                DateTimeComponents.Day => new DateTime(d.Year, d.Month, d.Day, 0, 0, 0, d.Kind),
                _ => throw new InvalidOperationException(),
            };
        }
    }
}
