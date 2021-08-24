using Amazon.DynamoDBv2.DocumentModel;
using BLambda.HolaMundo.Domain;
using BLambda.HolaMundo.Domain.TemperatureLog;
using Ddd.Abstructions.Domain;
using Ddd.DynamoDb;
using System;
using System.Globalization;
using System.Linq;

namespace BLambda.HolaMundo.Data
{
    internal sealed class TemperatureLogDbContext : IDynamoDbContext
    {
        public Document ToDocument<T>(T entity) where T : /*DynamoDbEntity<TPK, TSK>,*/ IAggregateRoot
        {
            _ = entity ?? throw new ArgumentNullException($"{nameof(entity)} cannot be null");

            return entity switch
            {
                LocationStat locationStat => new Document
                {
                    { "PK", locationStat.Location },
                    { "SK", nameof(LocationStat) },
                    { "Type", nameof(LocationStat) },
                    { "T", locationStat.Cur },
                    { "Date", locationStat.Date },
                    { "Stat", new Document {
                        { "Min", locationStat.Stat?.Min },
                        { "Max", locationStat.Stat?.Max },
                        { "Avg", locationStat.Stat?.Avg }
                    }},
                },
                YearStat stat => new Document
                {
                    { "PK", stat.Location },
                    { "SK", $"{nameof(YearStat)}#{stat.Year}" },
                    { "Type", nameof(YearStat) },
                    { "T", stat.Stat.Avg },
                    { "Year", stat.Year },
                    { "Stat", new Document {
                        { "Min", stat.Stat.Min },
                        { "Max", stat.Stat.Max },
                        { "Avg", stat.Stat.Avg }
                    }},
                },
                MonthStat stat => new Document
                {
                    { "PK", stat.Location },
                    { "SK", $"{nameof(MonthStat)}#{stat.Year}-{(int)stat.Month:00}" },
                    { "Type", nameof(MonthStat) },
                    { "T", stat.Stat.Avg },
                    { "Month", $"{stat.Month}" },
                    { "Year", stat.Year },
                    { "Stat", new Document {
                        { "Min", stat.Stat.Min },
                        { "Max", stat.Stat.Max },
                        { "Avg", stat.Stat.Avg }
                    }},
                },
                DayStat stat => new Document
                {
                    { "PK", stat.Location },
                    { "SK", $"{nameof(DayStat)}#{stat.Date:yyyy-MM-dd}" },
                    { "Type", nameof(DayStat) },
                    { "T", stat.Stat.Avg },
                    { "Stat", new Document {
                        { "Min", stat.Stat.Min },
                        { "Max", stat.Stat.Max },
                        { "Avg", stat.Stat.Avg }
                    }},
                },
                _ => throw new NotImplementedException($"There is no {typeof(T)} to Document mapper implemented.")
            };
        }

        public T FromDocument<T>(Document document) where T : notnull, IAggregateRoot
        {
            _ = document ?? throw new ArgumentNullException($"{nameof(document)} cannot be null");
            //if (document == null) return default;

            var result = this.FromDocument(default(TokenOf<T>), document);
            return (T)result;
        }

        private object FromDocument<T>(TokenOf<T> tokenized, Document document) where T : notnull 
        {
            return tokenized switch
            {
                TokenOf<LocationStat> => new LocationStat(
                    location: document["PK"],
                    date: document["Date"].AsDateTime(),
                    cur: document["T"].AsDouble()
                )
                {
                    Stat = new(
                        document["Stat"].AsDocument()["Min"].AsDouble(), 
                        document["Stat"].AsDocument()["Min"].AsDouble(), 
                        document["Stat"].AsDocument()["Min"].AsDouble()
                    )
                },

                TokenOf<YearStat> => new YearStat(
                    location: document["PK"], 
                    year: document["SK"].StripKey('#').AsInt(),
                    stat: new(
                        document["Stat"].AsDocument()["Min"].AsDouble(),
                        document["Stat"].AsDocument()["Min"].AsDouble(),
                        document["Stat"].AsDocument()["Min"].AsDouble()
                    )
                ),

                TokenOf<MonthStat> => Map(() =>
                {
                    var date = document["SK"].StripKey('#').AsDateTime("yyyy-MM");
                    return new MonthStat(
                        location: document["PK"],
                        month: (Month)date.Month,
                        year: date.Year,
                        stat: new(
                            document["Stat"].AsDocument()["Min"].AsDouble(),
                            document["Stat"].AsDocument()["Min"].AsDouble(),
                            document["Stat"].AsDocument()["Min"].AsDouble()
                        )
                    );
                }),

                TokenOf<DayStat> => new DayStat(
                    location: document["PK"],
                    date: document["SK"].StripKey('#').AsDateTime("yyyy-MM-dd"),
                    stat: new(
                        document["Stat"].AsDocument()["Min"].AsDouble(),
                        document["Stat"].AsDocument()["Min"].AsDouble(),
                        document["Stat"].AsDocument()["Min"].AsDouble()
                    )
                ),
                _ => throw new NotImplementedException($"There is no Document to {typeof(T)} mapper implemented.")
            };
        }

        private static T Map<T>(Func<T> f) => f();
        private struct TokenOf<T> { };        
    }

    internal static class DynamoDBEntryExt 
    {
        public static DateTime AsDateTime(this string p, string template)
        {
            return DateTime.ParseExact(p, template, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal);
        }

        public static int AsInt(this string p)
        {
            return int.Parse(p);
        }

        public static string StripKey(this DynamoDBEntry p, char separator = default)
        {
            var source = p.AsString();
            if (separator != default)
            {
                if (!source.Contains(separator))
                {
                    throw new InvalidOperationException($"source: '{source}' does not contain separator '{separator}'");
                }
                source = source.Split(separator).Last();
            }

            return source;
        }
    }
}
