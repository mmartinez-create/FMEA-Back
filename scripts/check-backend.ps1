$ErrorActionPreference = 'Stop'
Write-Host 'Checking FMEA Manager API...' -ForegroundColor Cyan
$response = Invoke-RestMethod -Uri 'http://localhost:5170/api/health'
$response | ConvertTo-Json -Depth 5
