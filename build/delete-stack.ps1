param ($profile='default', $domain='BLambda')

$appStackName = "$domain`Stack"

Write-Host "Set profile..."
setx AWS_PROFILE $profile | Out-Null
setx SAM_CLI_TELEMETRY 0 | Out-Null
./refreshenv.ps1


$appBucketName=$(aws cloudformation describe-stacks `
	--stack-name $appStackName `
	--query "Stacks[0].Outputs[?OutputKey=='AppBucketName'].OutputValue" `
	--output text `
) `

#echo AppBucketName=$AppBucketName
if ($appBucketName -ne $null)
{
	aws --profile $profile `
			s3 rb --force s3://$appBucketName `
}



$shallBucketName=$(aws cloudformation describe-stacks `
	--stack-name $appStackName `
	--query "Stacks[0].Outputs[?OutputKey=='ShallBucketName'].OutputValue" `
	--output text `
) `

if ($shallBucketName -ne $null)
{
	aws --profile $profile `
			s3 rb --force s3://$shallBucketName `
}



aws --profile $profile `
		cloudformation delete-stack `
			--stack-name $appStackName `
