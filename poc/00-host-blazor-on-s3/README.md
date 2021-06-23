# Assumption #0 Blazor can be hosted on S3

Here is just "Hello world" Blazor app that will be deployed on S3.

## In order to prove

you should
1. Install VS
2. Install AWS Toolkit
3. Create AWS Account
4. Add IAM 'dev' with permissions
```
{
    "Version": "2012-10-17",
    "Statement": [
        {
            "Effect": "Allow",
            "Action": "cloudformation:*",
            "Resource": "*"
        },
        {
            "Effect": "Allow",
            "Action": "s3:*",
            "Resource": "*"
        }
    ]
}
```
5. create local profile for 'blambda-dev' (use aws_access_key_id and aws_secret_access_key from 'dev')
[Named profiles](https://docs.aws.amazon.com/cli/latest/userguide/cli-configure-profiles.html)
6. fire deploy-stack.ps1
7. browse http://[your-domain].s3-website.eu-central-1.amazonaws.com

## TODO:
1. routing/redirections for non root pages. like this http://[your-domain].s3-website.eu-central-1.amazonaws.com/counter
2. authentication
3. deny bucket listing (https://[your-domain].s3.amazonaws.com)
