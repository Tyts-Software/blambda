using Amazon.CDK;
using Amazon.CDK.AWS.APIGatewayv2;
using Amazon.CDK.AWS.APIGatewayv2.Integrations;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.Logs;
using System.Collections.Generic;

namespace BLambda.Infrastructure.Mainstream
{
    internal class HolaMundoStackProps : StackProps
    {
        public string Domain { get; set; }
        public string SubDomain { get; set; }
        public string LogLevel { get; set; }
    }

    internal class HolaMundoStack : NestedStack
    {
        public HolaMundoStack(Construct scope, string id, HolaMundoStackProps props) : base(scope, id)
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

            var api = new HttpApi(this, "WebApi", new HttpApiProps
            {
                ApiName = "HolaMundoApi",
                Description = "BLambda HolaMundoApi",

                CreateDefaultStage = true,
                
                //DefaultIntegration = new LambdaProxyIntegration(new LambdaProxyIntegrationProps
                //{
                //    Handler = webApiFunction,
                //    PayloadFormatVersion = PayloadFormatVersion.VERSION_2_0
                //})
            });

            var log = new LogGroup(this, "WebApiLog", new LogGroupProps
            {
                Retention = RetentionDays.ONE_WEEK
            });

            (api.DefaultStage.Node.DefaultChild as CfnStage).AccessLogSettings = new CfnStage.AccessLogSettingsProperty
            {
                DestinationArn = log.LogGroupArn,
                Format = "$context.identity.sourceIp - - [$context.requestTime] \"$context.httpMethod $context.routeKey $context.protocol\" $context.status $context.responseLength $context.requestId"
                // "Format": "{\"requestId\":\"$context.requestId\", \"ip\": \"$context.identity.sourceIp\", \"caller\":\"$context.identity.caller\", \"user\":\"$context.identity.user\",\"requestTime\":\"$context.requestTime\", \"routeKey\":\"$context.routeKey\", \"status\":\"$context.status\"}"
            };

            api.AddRoutes(new AddRoutesOptions
            {
                Path = "/{proxy+}",
                Methods = new[] { HttpMethod.ANY },
                Integration = new LambdaProxyIntegration(new LambdaProxyIntegrationProps
                {
                    Handler = webApiFunction,
                    PayloadFormatVersion = PayloadFormatVersion.VERSION_2_0
                })
            });


            Amazon.CDK.Tags.Of(this).Add("SERVICE", "blambda-hola");
            Amazon.CDK.Tags.Of(this).Add("TRIGGER", "gateway");

            new CfnOutput(scope, "HolaMundoApiUrl", new CfnOutputProps
            {
                Value = api.Url,
                Description = "HolaMundo WebAPI endpoint"
            });
        }
    }
}
