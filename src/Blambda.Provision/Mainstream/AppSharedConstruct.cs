using Amazon.CDK;
using Amazon.CDK.AWS.DynamoDB;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.S3;
using Amazon.CDK.AWS.SSM;

namespace BLambda.Provision.Mainstream
{
    internal sealed class AppSharedConstructProps
    {
        public string Domain { get; set; }
    }
    

    internal sealed class AppSharedConstruct : Construct
    {
        private readonly Table table;

        public AppSharedConstruct(Construct scope, AppSharedConstructProps props) : base(scope, "Shared")
        {
            var appBucket = new Bucket(scope, "AppBucket", new BucketProps
            {
                BucketName = props.Domain.ToLower(), // let's aws set Unique names ?? but what about table names
                PublicReadAccess = false,
                Versioned = true,

                // The default removal policy is RETAIN, which means that cdk destroy will not attempt to delete
                // the new bucket, and it will remain in your account until manually deleted. By setting the policy to
                // DESTROY, cdk destroy will attempt to delete the bucket, but will error if the bucket is not empty.
                RemovalPolicy = RemovalPolicy.DESTROY, // NOT recommended for production code
                BlockPublicAccess = BlockPublicAccess.BLOCK_ALL,
                Encryption = BucketEncryption.S3_MANAGED
            });


            table = new Table(scope, "WeatherForecast", new TableProps
            {
                //TableName = "WeatherForecast",
                PartitionKey = new Amazon.CDK.AWS.DynamoDB.Attribute
                {
                    Name = "PK",
                    Type = AttributeType.STRING
                },
                SortKey = new Amazon.CDK.AWS.DynamoDB.Attribute
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
                IndexName = "GSI1",
                PartitionKey = new Amazon.CDK.AWS.DynamoDB.Attribute
                {
                    Name = "SK",
                    Type = AttributeType.STRING
                },
                SortKey = new Amazon.CDK.AWS.DynamoDB.Attribute
                {
                    Name = "Data",
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

            Amazon.CDK.Tags.Of(appBucket)
                .Add("BUCKET", "app-bucket");


            //// Output values
            new CfnOutput(scope, "AppBucketName", new CfnOutputProps
            {                
                Value = appBucket.BucketName,
                ExportName = $"{props.Domain}:AppBucket"
            });

            
            new CfnOutput(scope, "WeatherForecastTableName", new CfnOutputProps{
                Value = table.TableName
            });

            TableName = table.TableName;

            //TableNameParameter = $"/{props.Domain}/WeatherForecastTable";
            //// AWS Systems Manager -> Parameter Store
            //new StringParameter(this, "WeatherForecastTableNameParameter", new StringParameterProps
            //{
            //    ParameterName = TableNameParameter,
            //    StringValue = table.TableName,
            //});

            


            new CfnOutput(scope, "WeatherForecastTableArn", new CfnOutputProps{
                Value = table.TableArn,
                ExportName = $"arn:{props.Domain}:WeatherForecastTable",
            });

            //// Export shared resources
            //this.ExportValue(appBucket.BucketName);
            //this.ExportValue(table.TableName);
        }

        //public string TableNameParameter { get; }

        public string TableName 
        {
            get => System.Environment.GetEnvironmentVariable("WeatherForecastTableName");
            set => System.Environment.SetEnvironmentVariable("WeatherForecastTableName", value);
        }

        public void GrantReadData(IGrantable function) => table.GrantReadData(function);
    }
}
