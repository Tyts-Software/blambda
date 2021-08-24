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
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System;
using System.IO.Compression;
using System.Net.Mime;
using System.Text;

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

                options.AllowEmptyInputInBodyModelBinding = true;
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
            

            // add data layer here
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

            // Register the Swagger services
            services.AddSwaggerDocument(d => {
                //d.SchemaProcessors.Add(new MarkAsRequiredIfNonNullableSchemaProcessor());

                d.RequireParametersWithoutDefault = true;
                //d.AllowReferencesWithProperties = true;
            });

            // Health check 
            services.AddHealthChecks();

            // Responce compression
            services
                .Configure<BrotliCompressionProviderOptions>(options =>
                {
                    options.Level = CompressionLevel.Optimal;
                })
                .Configure<GzipCompressionProviderOptions>(options =>
                {
                    options.Level = CompressionLevel.Optimal;
                })
                .AddResponseCompression(options =>
                {
                    options.EnableForHttps = true; 
                    options.Providers.Add<BrotliCompressionProvider>();
                    options.Providers.Add<GzipCompressionProvider>();
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(ILoggerFactory logFactory, IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Enable compression
            app.UseResponseCompression();

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

            // Matches request to an endpoint.
            app.UseRouting();

            //app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                //// Configure the Health Check endpoint and require an authorized user.
                endpoints.MapHealthChecks("/healthz");//.RequireAuthorization();

                endpoints.MapControllers();
                
                // Configure another endpoint, no authorization requirements.
                endpoints.MapGet("/", async context =>
                {                    
                    context.Response.ContentType = "text/plain; charset=utf-8";
                    await context.Response.WriteAsync("¡Hola Mundo! BLambda API", Encoding.UTF8);
                });
            });
        }
    }
}
