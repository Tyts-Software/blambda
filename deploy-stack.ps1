param ($profile='blambda-dev', $domain='blambda', $configuration='Release')

# const
$deploy = ".\_deploy"
$deployPackages = "$deploy\packages"
$deployShall = "$deploy\shall"
$willPackage = "$deployPackages\BLambda.Will.zip"
$holaPackage = "$deployPackages\BLambda.HolaMundo.zip"
$templates = "$deploy\templates"

$shall = ".\src\BLambda.Shall"
$will = ".\src\BLambda.Will"
$hola = ".\src\BLambda.HolaMundo"

Write-Host "Set profile..."
setx AWS_PROFILE $profile | Out-Null
setx SAM_CLI_TELEMETRY 0 | Out-Null
./refreshenv.ps1


#### Re-Build projects
#

#clean up binaries
Write-Host "Cleaning up binaries..."
Get-ChildItem .\ -include bin,obj -Recurse | ForEach-Object ($_) { Remove-Item $_.FullName -Force -Recurse  -ErrorAction SilentlyContinue | Out-Null }
Remove-Item -LiteralPath $templates -Force -Recurse  -ErrorAction SilentlyContinue | Out-Null

### rebuild Web API (dotnet publish inside) 
dotnet lambda package `
	--project-location $will `
	--configuration "Release" `
	--framework "net5.0" `
	--msbuild-parameters "--self-contained true" `
	--package-type "zip" `
	--output-package $willPackage `

### rebuild HolaMundo API (dotnet publish inside)  TEMP!!!
dotnet lambda package `
	--project-location $hola `
	--configuration "Release" `
	--framework "netcoreapp3.1" `
	--msbuild-parameters "--self-contained false" `
	--package-type "zip" `
	--output-package $holaPackage `
#
### Build & publish UI locally
## create folder to publish
#New-Item -Path $deployShall -ItemType Directory -Force
## rebuild and publish UI
#dotnet publish $shall --output $deployShall -c $configuration 


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
			--context domain=$domain `
			--context shall-subdomain=shall `
			--context will-subdomain=will `
			--context will-package=$willPackage `
			--context hola-package=$holaPackage `



#### Upload Shall to S3
#Write-Host "Waiting UI bucket is created..."
#$uiBucket = "shall.$domain"
#aws s3api wait bucket-exists --bucket $uiBucket
#Push-Location -Path "$deployShall\wwwroot"
#aws s3 sync . s3://$uiBucket
#Pop-Location

