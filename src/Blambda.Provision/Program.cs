using Amazon.CDK;
using System;

namespace BLambda.Provision
{
    sealed class Program
    {
        public static void Main(string[] args)
        {
            var appName = "BLambda";
            var app = new App();

            var domain = ((string)app.Node.TryGetContext("domain") ?? appName).ToLower();
            appName = domain != appName.ToLower()
                ? appName += $"{char.ToUpper(domain[0])}{domain[1..]}{appName}Stack" 
                : $"{appName}Stack";

            var account = System.Environment.GetEnvironmentVariable("CDK_DEFAULT_ACCOUNT");
                    //?? throw new ArgumentNullException("CDK_DEFAULT_ACCOUNT");

            var region = System.Environment.GetEnvironmentVariable("CDK_DEFAULT_REGION");
                    //?? throw new ArgumentNullException("CDK_DEFAULT_REGION");

            Console.WriteLine($"account: {account}");
            Console.WriteLine($"region: {region}");
            Console.WriteLine($"domain: {domain}");

            new AppStack(app, appName, new StackProps
            {
                // For more information, see https://docs.aws.amazon.com/cdk/latest/guide/environments.html
                Env = new Amazon.CDK.Environment
                {
                    Account = account,
                    Region = region
                }
            });

            Tags.Of(app).Add("APP", appName);

            app.Synth();
        }
    }
}
