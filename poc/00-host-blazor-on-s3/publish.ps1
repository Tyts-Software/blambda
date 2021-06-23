param ($profile='blambda-dev', $domain='blambda', $buildConfig='Release')

# Publish frontend
$deploy = ".\_deploy\shell"
$shell = ".\src\Tyts.Shall"

New-Item -Path $deploy -ItemType Directory -Force
dotnet publish $shell --output $deploy -c $buildConfig 

# Upload frontend to S3
Push-Location -Path "$deploy\wwwroot"
aws --profile $profile `
		s3 sync . s3://$domain `

Pop-Location #-PassThru