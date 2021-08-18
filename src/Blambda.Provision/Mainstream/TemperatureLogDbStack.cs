using Amazon.CDK;
using Amazon.CDK.AWS.DynamoDB;
using Amazon.CDK.AWS.SSM;

namespace BLambda.Provision.Mainstream
{
    internal sealed class TemperatureLogDb : Construct
    {
        public string TableNameParameter { get; }

        public TemperatureLogDb(Construct scope, string id, AppSharedConstructProps props) : base(scope, id)
        {
            var table = new Table(scope, "TemperatureLogTable", new TableProps
            {
                //TableName = "TemperatureLog",
                PartitionKey = new Attribute
                {
                    Name = "PK",
                    Type = AttributeType.STRING
                },
                SortKey = new Attribute
                {
                    Name = "SK",
                    Type = AttributeType.STRING
                },

                //BillingMode = BillingMode.PAY_PER_REQUEST,
                BillingMode = BillingMode.PROVISIONED,
                ReadCapacity = 1,
                WriteCapacity = 1,

                //Stream = StreamViewType.NEW_IMAGE,

                // The default removal policy is RETAIN, which means that cdk destroy will not attempt to delete
                // the new table, and it will remain in your account until manually deleted. By setting the policy to 
                // DESTROY, cdk destroy will delete the table (even if it has data in it)
                RemovalPolicy = RemovalPolicy.DESTROY

            });

            table.AddGlobalSecondaryIndex(new GlobalSecondaryIndexProps
            {
                IndexName = "TemperatureIndex",
                PartitionKey = new Attribute
                {
                    Name = "SK",
                    Type = AttributeType.STRING
                },
                SortKey = new Attribute
                {
                    Name = "T",
                    Type = AttributeType.NUMBER
                },

                ProjectionType = ProjectionType.ALL,
                ReadCapacity = 1,
                WriteCapacity = 1
            });

            table.AddGlobalSecondaryIndex(new GlobalSecondaryIndexProps
            {
                IndexName = "TypeIndex",
                PartitionKey = new Attribute
                {
                    Name = "Type",
                    Type = AttributeType.STRING
                },

                ProjectionType = ProjectionType.ALL,
                ReadCapacity = 1,
                WriteCapacity = 1
            });

            //table.AddLocalSecondaryIndex(new LocalSecondaryIndexProps
            //{
            //    IndexName = "temperatureIndex",
            //    SortKey = new Amazon.CDK.AWS.DynamoDB.Attribute
            //    {
            //        Name = "Temperature",
            //        Type = AttributeType.NUMBER
            //    },
            //    ProjectionType = ProjectionType.ALL,
            //});

            ////configure auto scaling on table
            //const writeAutoScaling = table.autoScaleWriteCapacity({
            //minCapacity: 1,
            //  maxCapacity: 2,
            //});

            ////scale up when write capacity hits 75%
            //        writeAutoScaling.scaleOnUtilization({
            //        targetUtilizationPercent: 75,
            //});

            ////scale up at 9 o'clock in the morning
            //writeAutoScaling.scaleOnSchedule('scale-up', {
            //        schedule: appautoscaling.Schedule.cron({ hour: '9', minute: '0'}),
            //  minCapacity: 2,
            //});

            ////scale down in the afternoon
            //writeAutoScaling.scaleOnSchedule('scale-down', {
            //        schedule: appautoscaling.Schedule.cron({ hour: '14', minute: '0'}),
            //  maxCapacity: 2,
            //});



            //table.GrantReadWriteData(new AccountRootPrincipal());

            //Tags.Of(appBucket).Add("BUCKET", "app-bucket");

            // Output values

            new CfnOutput(scope, "TemperatureLogTableName", new CfnOutputProps
            {
                Value = table.TableName
            });
            TableNameParameter = new StringParameter(scope, "TemperatureLogTableNameParameter", new StringParameterProps
            {
                ParameterName = $"/{props.Domain}/TemperatureLogTable",
                StringValue = table.TableName,
            }).ParameterName;

            new CfnOutput(scope, "TemperatureLogTableArn", new CfnOutputProps
            {
                Value = table.TableArn,
                //ExportName = $"arn:{props.Domain}:TemperatureLogTable",
            });
        }
    }
}
