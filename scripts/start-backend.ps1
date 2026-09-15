$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
Set-Location $root

dotnet restore
dotnet build
dotnet run --project .\FMEA-Api\FMEA-Api.csproj --launch-profile http
