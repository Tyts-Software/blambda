using Amazon.CDK;
using Amazon.CDK.AWS.APIGatewayv2;
using Amazon.CDK.AWS.APIGatewayv2.Integrations;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.Lambda.EventSources;
using Amazon.CDK.AWS.Logs;
using System.Collections.Generic;

namespace BLambda.Provision.Mainstream
{
    internal class WillStackProps : StackProps
    {
        public string Domain { get; set; }
        public string SubDomain { get; set; }
        public string LogLevel { get; set; }
    }

    internal sealed class WillStack : NestedStack
    {
        public WillStack(Construct scope, string id, WillStackProps props) : base(scope, id)
        {
            var domain = props.Domain ?? (string)this.Node.TryGetContext("domain") ?? "blambda";
            var subDomain = props.SubDomain ?? (string)this.Node.TryGetContext("will-subdomain") ?? "will";
            var lambdaPackage = (string)this.Node.TryGetContext("will-package") ?? "BLambda.Will.zip";
            var apiDomain = $"{subDomain}.{domain}";

            var logLevel = props.LogLevel ?? (string)this.Node.TryGetContext("log-level") ?? "Warning";

            var webApiFunction = new Function(this, "WebApiFunction", new FunctionProps
            {
                Runtime = Runtime.PROVIDED,
                Code = Code.FromAsset(lambdaPackage),
                Handler = "not_required_for_custom_runtime",
                MemorySize = 128,
                Timeout = Duration.Seconds(10),
                //ReservedConcurrentExecutions = 5, // optional, reserved concurrency limit for this function. By default, AWS uses account concurrency limit
                //Tracing =  Tracing.PASS_THROUGH, // optional, overwrite, can be 'Active' or 'PassThrough'
                //CurrentVersionOptions = new VersionOptions
                //{
                //    ProvisionedConcurrentExecutions = 5 // warm instances
                //},

                //Events = new[]
                //{
                //    new ApiEventSource("ANY", "/{proxy+}", new Amazon.CDK.AWS.APIGateway.MethodOptions
                //    { 
                        
                //    })
                //},

                Role = null,

                Environment = new Dictionary<string, string> {
                    { "LOG_LEVEL", logLevel }
                }
            });

            var api = new HttpApi(this, "WebApi", new HttpApiProps
            {
                ApiName = "HttpApi",
                CreateDefaultStage = true,
                Description = "BLambda WebApi",

                //  CorsPreflight = {
                //      allowHeaders:[
                //       'Content-Type',
                //    'X-Amz-Date',
                //    'Authorization',
                //    'X-Api-Key',
                //  ],
                //  allowMethods:[
                //   CorsHttpMethod.OPTIONS,
                //   CorsHttpMethod.GET,
                //   CorsHttpMethod.POST,
                //   CorsHttpMethod.PUT,
                //   CorsHttpMethod.PATCH,
                //   CorsHttpMethod.DELETE,
                // ],
                //  allowCredentials: true,
                //  allowOrigins:['http://localhost:3000'],
                //}

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


            Amazon.CDK.Tags.Of(this).Add("SERVICE", "blambda-will");
            Amazon.CDK.Tags.Of(this).Add("TRIGGER", "gateway");

            new CfnOutput(scope, "WebApiUrl", new CfnOutputProps
            {
                Value = api.Url
            });
        }
    }
}
