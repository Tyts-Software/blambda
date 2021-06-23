param ($profile='blambda-dev', $domain='blambda')

### Create Stack
aws --profile $profile `
		cloudformation create-stack `
			--stack-name "$domain-stack" `
			--template-body file://./src/SetupS3Template/create-stack.template `
			--parameters ParameterKey=RootDomainName,ParameterValue=$domain `
						 #ParameterKey=Parm2,ParameterValue=test2 `


### Publish frontend
$deploy = ".\_deploy\shell"
$shell = ".\src\Tyts.Shall"

# create folder to publish locally
New-Item -Path $deploy -ItemType Directory -Force
# rebuild and publish 
dotnet publish $shell --output $deploy -c Release 

# wait until bucket exist
Write-Host "Waiting bucket is created..."
aws --profile $profile `
		s3api wait bucket-exists `
			--bucket $domain

# upload frontend to S3
Push-Location -Path "$deploy\wwwroot"
aws --profile $profile `
		s3 sync . s3://$domain `

Pop-Location
