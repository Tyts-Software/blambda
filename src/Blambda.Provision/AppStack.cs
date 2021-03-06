using Amazon.CDK;
using BLambda.Provision.Mainstream;

namespace BLambda.Provision
{    
    public class AppStack : Stack
    {
        internal AppStack(Construct scope, string id, StackProps props = null) : base(scope, id, props)
        {
            var Domain = (string)this.Node.TryGetContext("domain") ?? "blambda";

            var shared = new AppSharedConstruct(this, new AppSharedConstructProps
            {
                Domain = Domain
            });

            var tLogDb = new TemperatureLogDb(this, "TemperatureLogDb", new AppSharedConstructProps
            {
                Domain = Domain
            });

            ///// Mainstream

            //// Shall
            //var shall = new ShallStack(this, "ShallStack");

            //// Will (custom runtime)
            //var will = new WillStack(this, "WillStack", new WillStackProps
            //{
            //    Domain = Domain,
            //    SubDomain = "will",
            //});

            // HolaMundo API
            var hola = new HolaMundoStack(this, "HolaMundoStack", new HolaMundoStackProps
            {
                Domain = Domain,
                SubDomain = "hola",

                WeatherForecastDb = shared,
                TemperatureLogDb = tLogDb
            });

            ///// Pipelines
        }
    }
}
