using Amazon.CDK;
using Amazon.CDK.AWS.APIGateway;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.Logs;
using System;
using System.Collections.Generic;

namespace BLambda.Provision.Mainstream
{
    internal class RestApiStackProps : StackProps
    {
        public string Domain { get; set; }
        public string SubDomain { get; set; }
        public string LogLevel { get; set; }
    }

    [Obsolete("Use HttpApi instead")]
    internal sealed class RestApiStack : NestedStack
    {
        public RestApiStack(Construct scope, string id, RestApiStackProps props) : base(scope, id)
        {
            var domain = props.Domain ?? (string)this.Node.TryGetContext("domain") ?? "blambda";
            var subDomain = props.SubDomain ?? (string)this.Node.TryGetContext("hola-subdomain") ?? "will";
            var lambdaPackage = (string)this.Node.TryGetContext("hola-package") ?? "BLambda.HolaMundo.zip";
            var apiDomain = $"{subDomain}.{domain}";

            var logLevel = props.LogLevel ?? (string)this.Node.TryGetContext("log-level") ?? "INFO";

            var webApiFunction = new Function(this, "WebApiFunction", new FunctionProps
            {
                Runtime = Runtime.DOTNET_CORE_3_1,
                Code = Code.FromAsset(lambdaPackage),
                Handler = "BLambda.HolaMundo::BLambda.HolaMundo.LambdaEntryPoint::FunctionHandlerAsync",
                MemorySize = 128,
                Timeout = Duration.Seconds(10),
                Role = null,

                Environment = new Dictionary<string, string>{
                    { "LOG_LEVEL", logLevel }
                }
            });

            var api = new RestApi(this, "WebApi", new RestApiProps
            {
                RestApiName = "RestApi",
                Description = "BLambda RestApi",

                Deploy = true,                
                DeployOptions = new StageOptions 
                { 
                    AccessLogDestination = new LogGroupLogDestination(new LogGroup(this, "WebApiLog", new LogGroupProps
                    {
                        Retention = RetentionDays.ONE_WEEK
                    })),
                    AccessLogFormat = AccessLogFormat.Custom("$context.identity.sourceIp - - [$context.requestTime] \"$context.httpMethod $context.routeKey $context.protocol\" $context.status $context.responseLength $context.requestId")
                },

                DefaultIntegration = new LambdaIntegration(webApiFunction)
            });

            api.Root.AddMethod("ANY");
            api.Root.AddProxy();

            //api.AddRoutes(new AddRoutesOptions
            //{
            //    Path = "/{proxy+}",
            //    Methods = new[] { HttpMethod.ANY },
            //    Integration = new LambdaProxyIntegration(new LambdaProxyIntegrationProps
            //    {
            //        Handler = webApiFunction,
            //        PayloadFormatVersion = PayloadFormatVersion.VERSION_2_0
            //    })
            //});


            Amazon.CDK.Tags.Of(this).Add("SERVICE", "blambda-hola");
            Amazon.CDK.Tags.Of(this).Add("TRIGGER", "gateway");

            new CfnOutput(scope, "RestApiUrl", new CfnOutputProps
            {
                Value = api.Url,
                Description = "HolaMundo WebAPI endpoint"
            });
        }
    }
}
