# Assumption #1 It should be Weather Forecast API Lambda

Lets publish Weather Forecast API and will use it in Blazor UI.

## Nothing to prove

but what will do
1. Create Web API project from AWS VS tookit lambda template (BLambda.Will) with custom runtime (.net5) but it won't be Dockerized
2. Call Weather Forecast API from Blazor UI (BLambda.Shall)
3. Add some kind of architecture abstractions thinking about Domain Driven Design (DDD) inside the microservices
4. Update IAM 'dev' permissions
```
{
    "Version": "2012-10-17",
    "Statement": [
        {
            "Effect": "Allow",
            "Action": [
                "cloudformation:*",
                "cloudwatch:*",
                "logs:*",
                "lambda:*",
                "apigateway:*"
            ],
            "Resource": "*",
            "Condition": {
                "StringEquals": {
                    "aws:RequestedRegion": "eu-central-1"
                }
            }
        },
        {
            "Effect": "Allow",
            "Action": [
                "iam:*",
                "cloudfront:*",
                "s3:*"
            ],
            "Resource": "*"
        }
    ]
}
```
5. Create clone of the Forecast API (BLambda.HolaMundo) but with netcoreapp3.1, currently it is the newest version of .core supported by AWS (later you can play with X-Ray, trace and compare cold, warm starts ...)
6. Use templates (BLambda.Templates) to describe and SAM CLI to deploy on AWS all this stuff (not really SAM CLI but.. it is an Infrastructure As Code, amigo!)

## TODO:
- [ ] use AWS CDK - true Infrastructure As Code 
- [ ] add authentication to API
