using BLambda.HolaMundo.Domain.TemperatureLog;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BLambda.HolaMundo.Helper
{
    public class StatInputFormatter : TextInputFormatter
    {
        public StatInputFormatter()
        {
            SupportedMediaTypes.Add(MediaTypeNames.Application.Json);

            SupportedEncodings.Add(Encoding.UTF8);
            SupportedEncodings.Add(Encoding.Unicode);
        }

        protected override bool CanReadType(Type type)
        {
            return type == typeof(IStat) && base.CanReadType(type);
            //return typeof(IStat).IsAssignableFrom(type);
        }

        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding encoding)
        {
            var httpContext = context.HttpContext;
            var serviceProvider = httpContext.RequestServices;
            var logger = serviceProvider.GetRequiredService<ILogger<StatInputFormatter>>();

            IStat? stat = default;
            var type = string.Empty;
            var documentOptions = new JsonDocumentOptions
            {
                CommentHandling = JsonCommentHandling.Skip
            };
            var serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            try
            {
                using var reader = new StreamReader(httpContext.Request.Body, encoding);
                var json = await reader.ReadToEndAsync();
                
                using JsonDocument document = JsonDocument.Parse(json, documentOptions);
                JsonElement root = document.RootElement;
                
                if (root.TryGetProperty("type", out JsonElement typeElement))
                {
                    type = typeElement.GetString();
                    
                    stat = type switch
                    {
                        nameof(DayStat) => JsonSerializer.Deserialize<DayStat>(json, serializerOptions),
                        nameof(MonthStat) => JsonSerializer.Deserialize<MonthStat>(json, serializerOptions),
                        nameof(YearStat) => JsonSerializer.Deserialize<YearStat>(json, serializerOptions),
                        _ => JsonSerializer.Deserialize<LocationStat>(json, serializerOptions)
                    };
                }

                return stat != default
                    ? await InputFormatterResult.SuccessAsync(stat)
                    : throw new NotSupportedException($"Cannot deserialize {type} {nameof(IStat)} from {json}");
            }
            catch(Exception e)
            {
                logger.LogDebug(e, $"Cannot deserialize {nameof(IStat)}");
                return await InputFormatterResult.FailureAsync();
            }
        }        
    }
}
