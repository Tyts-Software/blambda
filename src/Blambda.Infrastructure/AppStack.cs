using Amazon.CDK;
using Amazon.CDK.AWS.S3;
using BLambda.Infrastructure.Mainstream;

namespace BLambda.Infrastructure
{    
    public class AppStack : Stack
    {
        internal AppStack(Construct scope, string id, StackProps props = null) : base(scope, id, props)
        {
            var Domain = (string)this.Node.TryGetContext("domain") ?? "blambda";
               
            var siteBucket = new Bucket(this, "AppBucket", new BucketProps
            {
                BucketName = Domain,
                PublicReadAccess = false,
                Versioned = true,

                // The default removal policy is RETAIN, which means that cdk destroy will not attempt to delete
                // the new bucket, and it will remain in your account until manually deleted. By setting the policy to
                // DESTROY, cdk destroy will attempt to delete the bucket, but will error if the bucket is not empty.
                RemovalPolicy = RemovalPolicy.DESTROY, // NOT recommended for production code
                BlockPublicAccess = new BlockPublicAccess(new BlockPublicAccessOptions
                {
                    BlockPublicAcls = true,
                    BlockPublicPolicy = true,
                    IgnorePublicAcls = true,
                    RestrictPublicBuckets = true
                }),
                Encryption = BucketEncryption.S3_MANAGED
            });

            Amazon.CDK.Tags.Of(siteBucket).Add("BUCKET", "app-bucket");

            new CfnOutput(this, "AppBucketName", new CfnOutputProps
            {
                Value = siteBucket.BucketName
            });

            ///// Mainstream
            
            // Shall
            var shall = new ShallStack(this, $"{Domain}-ShallStack");

            // Will
            var will = new WillStack(this, $"{Domain}-WillStack", new WillStackProps
            {
                Domain = Domain,
                SubDomain = "will",
                LogLevel = "INFO"
            });

            var hola = new HolaMundoStack(this, $"{Domain}-HolaMundoStack", new HolaMundoStackProps
            {
                Domain = Domain,
                SubDomain = "hola",
                LogLevel = "INFO"
            });

            ///// Pipelines
        }
    }
}
