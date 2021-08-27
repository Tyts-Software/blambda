using Amazon.CDK;
using Amazon.CDK.AWS.S3;
using Amazon.CDK.AWS.CloudFront;
using Amazon.CDK.AWS.CloudFront.Origins;
using Amazon.CDK.AWS.IAM;

namespace BLambda.Provision.Mainstream
{
    internal sealed class ShallStack : NestedStack
    {
        public ShallStack(Construct scope, string id) : base(scope, id)
        {
            var Domain = (string)this.Node.TryGetContext("domain") ?? "blambda";
            var ShallSubDomain = (string)this.Node.TryGetContext("shall-subdomain") ?? "shall";
            var logLevel = (string)this.Node.TryGetContext("log-level") ?? "Warning";

            //var zone = HostedZone.FromLookup(this, "Zone", new HostedZoneProviderProps
            //{
            //    DomainName = props.DomainName
            //});

            var siteDomain = $"{ShallSubDomain}.{Domain}";
            //new CfnOutput(this, "Site", new CfnOutputProps
            //{
            //    Value = $"https://{siteDomain}"
            //});

            var shallBucket = new Bucket(this, "ShallBucket", new BucketProps
            {
                //BucketName = siteDomain, // let's aws set Unique names
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

            var oai = new OriginAccessIdentity(this, "OAI", new OriginAccessIdentityProps
            {
                Comment = "Access S3 bucket content only through CloudFront"
            });


            var policy = new PolicyStatement(new PolicyStatementProps
            {
                Effect = Effect.ALLOW,
                Principals = new[]
                {
                    oai.GrantPrincipal
                },
                Actions = new[] { "s3:GetObject" },
                Resources = new[]
                {
                    shallBucket.ArnForObjects("*"),
                    shallBucket.BucketArn
                },
            
            });

            shallBucket.AddToResourcePolicy(policy);

            //var certificateArn = new DnsValidatedCertificate(this, "SiteCertificate", new DnsValidatedCertificateProps
            //{
            //    DomainName = siteDomain,
            //    HostedZone = zone
            //}).CertificateArn;

            //new CfnOutput(this, "Certificate", new CfnOutputProps { Value = certificateArn });

            var distribution = new Distribution(this, "ShallDistribution", new DistributionProps 
            {
                Enabled = true,
                DefaultRootObject = "index.html",
                PriceClass = PriceClass.PRICE_CLASS_100,

                DefaultBehavior = new BehaviorOptions 
                { 
                    Origin = new S3Origin(shallBucket, new S3OriginProps 
                    { 
                        OriginAccessIdentity = oai
                    }),
                    ViewerProtocolPolicy =ViewerProtocolPolicy.REDIRECT_TO_HTTPS,
                    AllowedMethods = AllowedMethods.ALLOW_GET_HEAD_OPTIONS,
                    CachedMethods = CachedMethods.CACHE_GET_HEAD_OPTIONS,
                    CachePolicy = CachePolicy.CACHING_OPTIMIZED,
                    Compress = false                    
                },

                //GeoRestriction = GeoRestriction.Allowlist("US", "UK"),
                //Certificate = 
                
                ErrorResponses = new IErrorResponse[] 
                { 
                    new ErrorResponse
                    {
                        HttpStatus = 404,
                        ResponseHttpStatus = 200,
                        ResponsePagePath = "/index.html"
                    },
                    new ErrorResponse
                    {
                        HttpStatus = 403,
                        ResponseHttpStatus = 200,
                        ResponsePagePath = "/index.html"
                    }
                }
            });           

            //new ARecord(this, "SiteAliasRecord", new ARecordProps
            //{
            //    RecordName = siteDomain,
            //    Target = RecordTarget.FromAlias(new CloudFrontTarget(distribution)),
            //    Zone = zone
            //});

            //new BucketDeployment(this, "DeployWithInvalidation", new BucketDeploymentProps
            //{
            //    Sources = new ISource[] { Source.Asset("../site-contents") },
            //    DestinationBucket = shallBucket,
            //    Distribution = distribution,
            //    DistributionPaths = new string[] { "/*" }
            //});


            Amazon.CDK.Tags.Of(this).Add("SERVICE", "blambda-shall");
            Amazon.CDK.Tags.Of(this).Add("TRIGGER", "cloudfront");

            new CfnOutput(scope, "ShallBucketName", new CfnOutputProps
            {
                Value = shallBucket.BucketName,
                ExportName = $"{Domain}:ShallBucket"
            });

            new CfnOutput(scope, "CloudFrontDomain", new CfnOutputProps
            {
                Value = distribution.DistributionDomainName
            });
        }
    }
}
