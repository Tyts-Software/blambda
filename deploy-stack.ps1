param ($profile='blambda-dev', $domain='blambda', $configuration='Release')

# const
$deploy = ".\_deploy\shall"
$shall = ".\src\BLambda.Shall"
$will = ".\src\BLambda.Will"
$hola = ".\src\BLambda.HolaMundo"
$templates = ".\src\BLambda.Templates"

# var
$rootStack = "$domain-stack" 


Write-Host "Set profile..."
setx AWS_PROFILE $profile
setx SAM_CLI_TELEMETRY 0
./refreshenv.ps1


### CREATE Root stack
$stackInfo = $null;
$stackInfo = aws cloudformation describe-stacks --stack-name $rootStack
if ($stackInfo -eq $null)
{
	Write-Host "Root Stack does not exist, creating..."

	# on-failure DO_NOTHING ROLLBACK DELETE
	aws  cloudformation create-stack `
		--stack-name $rootStack `
		--template-body file://./src/BLambda.Templates/blambda-init.template `
		--on-failure DO_NOTHING `
		--capabilities CAPABILITY_AUTO_EXPAND CAPABILITY_IAM `
		--parameters ParameterKey=AppName,ParameterValue=$domain `
						 
	# wait until bucket exist
	Write-Host "Waiting app bucket is created..."
	aws s3api wait bucket-exists `
		--bucket $domain `
}

### Build & Prepare deployment packages

## rebuild Web API (dotnet publish inside) 
dotnet lambda package `
	--project-location $will `
	--config-file aws-lambda-tools-defaults.json `
	
## rebuild HolaMundo API (dotnet publish inside)  TEMP!!!
dotnet lambda package `
	--project-location $hola `
	--config-file aws-lambda-tools-defaults.json `

## Build & publish UI locally
# create folder to publish
New-Item -Path $deploy -ItemType Directory -Force
# rebuild and publish UI
dotnet publish $shall --output $deploy -c $configuration 


### Merge Nested Stacks
Write-Host "Merge Nested Stacks..."
Push-Location -Path $templates
aws  cloudformation package `
	--template-file blambda-stack.template `
	--output-template packed.json `
	--s3-bucket $domain `
	--s3-prefix deploy `
	--use-json `

Write-Host "Deploy Nested Stacks..."
aws cloudformation deploy `
	--template-file packed.json `
	--stack-name $rootStack `
	--s3-bucket $domain `
	--s3-prefix deploy `
	--capabilities CAPABILITY_AUTO_EXPAND CAPABILITY_IAM `
	--parameter-overrides AppName=$domain `
						  UIDomainName=$domain `
						  WebApiDomainName=api.$domain `
						  LogLevel=INFO `
	--tags APP=$domain `

Pop-Location


### Upload UI to S3
Write-Host "Waiting UI bucket is created..."
$uiBucket = "$domain-www"
aws s3api wait bucket-exists --bucket $uiBucket
Push-Location -Path "$deploy\wwwroot"
aws s3 sync . s3://$uiBucket

Pop-Location
