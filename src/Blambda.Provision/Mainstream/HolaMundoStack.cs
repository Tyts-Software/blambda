using Amazon.CDK;
using Amazon.CDK.AWS.APIGatewayv2;
using Amazon.CDK.AWS.APIGatewayv2.Integrations;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.Logs;
using Amazon.CDK.AWS.SSM;
using System.Collections.Generic;

namespace BLambda.Provision.Mainstream
{
    internal class HolaMundoStackProps : StackProps
    {
        public string Domain { get; set; }
        public string SubDomain { get; set; }
        public string LogLevel { get; set; }

        public AppSharedConstruct WeatherForecastDb { get; set; }
        public TemperatureLogDb TemperatureLogDb { get; set; }
    }

    internal sealed class HolaMundoStack : NestedStack
    {
        public HolaMundoStack(Construct scope, string id, HolaMundoStackProps props) : base(scope, id)
        {
            var domain = props.Domain ?? (string)this.Node.TryGetContext("domain") ?? "blambda";
            var subDomain = props.SubDomain ?? (string)this.Node.TryGetContext("hola-subdomain") ?? "will";
            var lambdaPackage = (string)this.Node.TryGetContext("hola-package") ?? "BLambda.HolaMundo.zip";
            var apiDomain = $"{subDomain}.{domain}";

            var logLevel = props.LogLevel ?? (string)this.Node.TryGetContext("log-level") ?? "Warning";

            var webApiFunction = new Function(this, "WebApiFunction", new FunctionProps
            {
                Runtime = Runtime.DOTNET_CORE_3_1,
                Code = Code.FromAsset(lambdaPackage),
                Handler = "BLambda.HolaMundo::BLambda.HolaMundo.LambdaEntryPoint::FunctionHandlerAsync",
                MemorySize = 128,
                Timeout = Duration.Seconds(30),
                Role = null, //auto generated

                //LogRetention = RetentionDays.ONE_DAY,
                
                Environment = new Dictionary<string, string>{
                    { "LOG_LEVEL", logLevel },

                    { "WeatherForecastTableName", props.WeatherForecastDb.TableName },
                    { "TemperatureLogTableName", props.TemperatureLogDb.TableName }
                }
            });

            // Grant DB access
            props.WeatherForecastDb.GrantReadData(webApiFunction);
            props.TemperatureLogDb.GrantReadWriteData(webApiFunction);

            // API Gateway
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


            // WebApi AccessLog in CloudWatch
            var log = new LogGroup(this, "WebApiLog", new LogGroupProps
            {
                LogGroupName = $"/{api.HttpApiName}-access-log/{webApiFunction.FunctionName}",
                Retention = RetentionDays.ONE_WEEK,
                RemovalPolicy = RemovalPolicy.DESTROY,
            });

            (api.DefaultStage.Node.DefaultChild as CfnStage).AccessLogSettings = new CfnStage.AccessLogSettingsProperty
            {
                DestinationArn = log.LogGroupArn,
                //Format = "$context.identity.sourceIp - - [$context.requestTime] \"$context.httpMethod $context.routeKey $context.protocol\" $context.status $context.responseLength $context.requestId"
                // "Format": "{\"requestId\":\"$context.requestId\", \"ip\": \"$context.identity.sourceIp\", \"caller\":\"$context.identity.caller\", \"user\":\"$context.identity.user\",\"requestTime\":\"$context.requestTime\", \"routeKey\":\"$context.routeKey\", \"status\":\"$context.status\"}"

                ////For HTTP APIs without a custom authorizer:
                Format = "{\"requestTime\":\"$context.requestTime\",\"requestId\":\"$context.requestId\",\"httpMethod\":\"$context.httpMethod\",\"path\":\"$context.path\",\"routeKey\":\"$context.routeKey\",\"status\":$context.status,\"responseLatency\":$context.responseLatency,\"integrationRequestId\":\"$context.integration.requestId\",\"functionResponseStatus\":\"$context.integration.status\",\"integrationLatency\":\"$context.integration.latency\",\"integrationServiceStatus\":\"$context.integration.integrationStatus\",\"ip\":\"$context.identity.sourceIp\",\"userAgent\":\"$context.identity.userAgent\",\"principalId\":\"$context.authorizer.principalId\"}",
                //// For HTTP APIs with a custom authorizer:
                //'{"requestTime":"$context.requestTime","requestId":"$context.requestId","httpMethod":"$context.httpMethod","path":"$context.path","routeKey":"$context.routeKey","status":$context.status,"responseLatency":$context.responseLatency,"integrationRequestId":"$context.integration.requestId","functionResponseStatus":"$context.integration.status","integrationLatency":"$context.integration.latency","integrationServiceStatus":"$context.integration.integrationStatus","authorizeResultStatus":"$context.authorizer.status","authorizerRequestId":"$context.authorizer.requestId","ip":"$context.identity.sourceIp","userAgent":"$context.identity.userAgent","principalId":"$context.authorizer.principalId"}'

            };

            // API Gateway + Lambda integration
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


            Tags.SetTag("SERVICE", "blambda-hola");
            Tags.SetTag("TRIGGER", "gateway");

            new CfnOutput(scope, "HolaMundoApiUrl", new CfnOutputProps
            {
                Value = api.Url,
                Description = "HolaMundo WebAPI endpoint"
            });
        }
    }
}
