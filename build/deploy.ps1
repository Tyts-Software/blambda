param ($profile='default', $domain='BLambda', $configuration='Release', 
	$logLevel='Information', # [Debug | Information | Warning]
	[switch]$hola,		# re-build & re-package HolaMundo lambda
	[switch]$will,		# re-build & re-package Will lambda
	[switch]$shall,		# re-publish UI
	[switch]$cleanup,	# clean up ALL binaries
	[switch]$prod		# skip all dev only things. use for real production deploymant
)

if ($prod) {
	$hola = $True
	$will = $True
	$shall = $True

	$cleanup = $True
	$logLevel='Warning'
}

# const
$deploy = "..\_deploy"
$deployPackages = "$deploy\packages"
$deployShall = "$deploy\shall"
$willPackage = "$deployPackages\BLambda.Will.zip"
$holaPackage = "$deployPackages\BLambda.HolaMundo.zip"
$templates = "$deploy\templates"

$shallPath = "..\src\BLambda.Shall"
$willPath = "..\src\BLambda.Will"
$holaPath = "..\src\BLambda.HolaMundo"

Write-Host "Set profile..."
setx AWS_PROFILE $profile | Out-Null
setx SAM_CLI_TELEMETRY 0 | Out-Null
./refreshenv.ps1

#### Re-Build projects
#
if ($cleanup) {
	#clean up binaries
	Write-Host "Cleaning up binaries..."
	Get-ChildItem ..\src\ -include bin,obj -Recurse | ForEach-Object ($_) { Remove-Item $_.FullName -Force -Recurse  -ErrorAction SilentlyContinue | Out-Null }
	Remove-Item -LiteralPath $deploy -Force -Recurse  -ErrorAction SilentlyContinue | Out-Null
}

if ($will) {
	### rebuild Web API (dotnet publish inside) 
	dotnet lambda package `
		--project-location $willPath `
		--configuration "Release" `
		--framework "net5.0" `
		--msbuild-parameters "--self-contained true" `
		--package-type "zip" `
		--output-package $willPackage `
}

if ($hola) {
	### rebuild HolaMundo API (dotnet publish inside)  TEMP!!!
	dotnet lambda package `
		--project-location $holaPath `
		--configuration "Release" `
		--framework "netcoreapp3.1" `
		--msbuild-parameters "--self-contained false" `
		--package-type "zip" `
		--output-package $holaPackage `	
}

if ($shall) {
	### Build & publish UI locally
	## create folder to publish
	New-Item -Path $deployShall -ItemType Directory -Force
	## rebuild and publish UI
	dotnet publish $shallPath --output $deployShall -c $configuration 

	## publish just few files in order to test deployment
	## is NOT for production
	if ($prod -eq $False)
	{
		Get-ChildItem $deployShall -File -Recurse -Force -exclude index.html,favicon.ico | ForEach-Object ($_) { Remove-Item $_.FullName -Force -ErrorAction SilentlyContinue | Out-Null }
	}
}


### CREATE stacks
Write-Host "Creating templates..."
Remove-Item -LiteralPath $templates -Force -Recurse  -ErrorAction SilentlyContinue | Out-Null

#cdk synth	--all --json `
#			--no-path-metadata --no-asset-metadata --no-version-reporting `
#			--outputs-file "$deploy\result.json" `
#			--output=$templates `
#			--context domain=$domain `
#			--context shall-subdomain=shall `
#			--context will-subdomain=will `
#			--context will-package=$willPackage `
#			--context hola-package=$holaPackage `

Write-Host "Deploying stack..."
cdk deploy	--all --json `
			--no-path-metadata --no-asset-metadata --no-version-reporting `
			--outputs-file "$deploy\result.json" `
			--output=$templates `
			--context log-level=$logLevel `
			--context domain=$domain `
			--context shall-subdomain=shall `
			--context will-subdomain=will `
			--context will-package=$willPackage `
			--context hola-package=$holaPackage `


			
### Upload Shall to S3
$uiBucketName=$(aws cloudformation describe-stacks `
	--stack-name "$domain`Stack" `
	--query "Stacks[0].Outputs[?OutputKey=='ShallBucketName'].OutputValue" `
	--output text `
) `

if ($uiBucketName -ne $null)
{
	Write-Host "Waiting UI bucket is created..."

	aws s3api wait bucket-exists --bucket $uiBucketName
	if((aws s3api head-bucket --bucket $uiBucketName) -eq $null) 
	{
		Push-Location -Path "$deployShall\wwwroot"
		aws s3 sync . s3://$uiBucketName
		Pop-Location
	}
}
