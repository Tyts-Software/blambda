param ($configuration='Release')

# API projects
$holaPath = "..\src\BLambda.HolaMundo"
$holaClientAbsPath = Resolve-Path "..\src\BLambda.HolaMundo.Client" | select -ExpandProperty Path

dotnet build $holaPath `
	-c $configuration `
	-p:ClientDir=$holaClientAbsPath `
	-p:GenerateApiClient=true `
