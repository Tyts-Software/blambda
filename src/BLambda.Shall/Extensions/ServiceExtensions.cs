using BLambda.HolaMundo.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLambda.Shall.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddApis(this IServiceCollection services, IConfiguration modules)
        {
            //services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            //services.AddHttpClient<IWeatherService, WeatherService>(x => x.BaseAddress = new Uri("http://localhost:8090"));
            //services.AddHttpClient<IWeatherService, WeatherService>(client => client.BaseAddress = new Uri(builder.Configuration["WeatherApiUrl"]));

            //foreach(var m in modules.GetChildren())
            //{
            //    foreach (var api in m.GetSection("Api").GetChildren())
            //    {
            //        api["BaseAddress"]
            //    }
            //}

            services.AddHttpClient<TemperatureClient>(c =>
            {
                c.BaseAddress = new Uri(modules["Weather:Api:BLambda.HolaMundo.Client:BaseAddress"]);
            });
        }
    }
}
