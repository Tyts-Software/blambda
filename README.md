# Blazor + AWS Lambda = BLambda

Modular, serverless aproach to build Progressive Web Applications (PWA) and not only, presented by WebAssembly hosted Blazor and served by AWS.

Welcome to proof of concept journey! 

## Getting Started

```
Goal: Find out one more solution for building serverless, modular PWA.
Main assumption: Let's say it would be Blazor for UI and AWS Lambda for BL.
```


## Assumptions

| Assumption | POC |
| ----- | ---- |
| [Assumption #0](https://github.com/Tyts-Software/blambda/tree/master/poc/00-host-blazor-on-s3) | It should be Blazor (WebAssembly) app hosted on S3 |
| [Assumption #1](https://github.com/Tyts-Software/blambda/tree/master/poc/01-blazor-use-http-api-lambda) | It should be Weather forecast API Lambda |


**IMPORTANT NOTE:** *Playing around with this POCs your AWS account will create and consume AWS resources, which **will cost money**. Be sure to shut down/remove all resources once you are finished to avoid ongoing charges to your AWS account (see instructions in delete-stack.ps1).*
