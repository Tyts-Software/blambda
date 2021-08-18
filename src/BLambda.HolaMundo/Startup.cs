using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using BLambda.HolaMundo.Data;
using BLambda.HolaMundo.Helper;
using Ddd.DynamoDb;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System;

namespace BLambda.HolaMundo
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration ?? throw new ArgumentNullException($"{nameof(configuration)}");
            Env = env ?? throw new ArgumentNullException($"{nameof(env)}");
        }

        #pragma warning disable CS8618
        public static IWebHostEnvironment Env  { get; private set; }
        public static IConfiguration Configuration { get; private set; }
        #pragma warning restore CS8618

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
                options.ValueProviderFactories.Add(new UpperCaseValueProviderFactory());

                //options.InputFormatters.Insert(0, new StatInputFormatter());
            })
            .AddJsonOptions(options => 
            {
                //options.JsonSerializerOptions.Converters.Add(new StatConverter());
            });

            services.AddRouting(options =>
            {
                options.ConstraintMap.Add("yyyy", typeof(YyyyRouteConstraint));
                options.ConstraintMap.Add("yyyy-MM", typeof(YyyyMmRouteConstraint));
                options.ConstraintMap.Add("yyyy-MM-dd", typeof(YyyyMmDdRouteConstraint));
            });

            services.AddDefaultAWSOptions(Configuration.GetAWSOptions());
            //services.AddAWSService<IAmazonS3>();
            services.AddAWSService<IAmazonDynamoDB>()          
                    .AddScoped<IAmazonDynamoDB, AmazonDynamoDBClient>()
                    .AddScoped<IDynamoDBContext, DynamoDBContext>();
            

            // adds data layer here
            services.AddRepositories();

            // logging
            services.AddLogging(config =>
            {
                config.AddAWSProvider(Configuration.GetAWSLoggingConfigSection());
                
                if (Env.IsDevelopment())
                {
                    AWSConfigs.LoggingConfig.LogTo = LoggingOptions.Console;
                }
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(ILoggerFactory logFactory, IApplicationBuilder app, IWebHostEnvironment env)
        {
            Util.ApplicationLogging.LoggerFactory = logFactory;

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var uiHost = env.IsDevelopment()
                ? "http://localhost:5000"
                : "http://blambda-www.s3-website.eu-central-1.amazonaws.com";
            app.UseCors(policy =>
                    policy.WithOrigins(uiHost)
                          .AllowAnyMethod()
                          .WithHeaders(HeaderNames.ContentType));

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("¡Hola Mundo! BLambda API");
                });
            });
        }
    }
}
