using BLambda.Shall.Service.Weather;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BLambda.Shall
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));

            //builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            //builder.Services.AddHttpClient<IWeatherService, WeatherService>(x => x.BaseAddress = new Uri("http://localhost:8090"));
            builder.Services.AddHttpClient<IWeatherService, WeatherService>(client => client.BaseAddress = new Uri(builder.Configuration["WeatherApiUrl"]));

            await builder.Build().RunAsync();
        }
    }
}
