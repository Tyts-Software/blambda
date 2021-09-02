using BLambda.HolaMundo.Data;
using BLambda.HolaMundo.Helper;
using Ddd.DynamoDb;
using Ddd.HttpApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text;

namespace BLambda.HolaMundo
{
    public class Startup : HttpApiApp
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env) 
        {
        }

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
                
                //options.ValueProviderFactories.Add(new UpperCaseValueProviderFactory());
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

            // add data layer here
            services
                .AddDynamoDb()
                .AddRepositories();

            // HttpApi              <-- BLambda's here
            services.AddHttpApi();

            // Health check 
            services.AddHealthChecks();            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(ILoggerFactory logFactory, IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseHttpApi(logFactory);           

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
