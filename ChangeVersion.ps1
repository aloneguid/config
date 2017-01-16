param(
   [String]
   $Version
)

Write-Host "version is $Version"

function Get-Json($RelPath)
{
   $path = "$PSScriptRoot\$RelPath"
   Get-Content $path | ConvertFrom-Json
}

function Set-Json($Json, $RelPath)
{
   $path = "$PSScriptRoot\$RelPath"
   $content = $Json | ConvertTo-Json -Depth 100
   $content | Set-Content -Path $path
}

$jsonMain = Get-Json "src\Config.Net\project.json"
$jsonAzure = Get-Json "src\Config.Net.Azure\project.json"
$jsonTests = Get-Json "src\Config.Net.Tests\project.json"

$jsonMain.version = $Version

$jsonAzure.version = $Version
$jsonAzure.dependencies."Config.Net" = $Version

$jsonTests.dependencies."Config.Net" = $Version
$jsonTests.dependencies."Config.Net.Azure" = $Version

Set-Json $jsonMain "src\Config.Net\project.json"
Set-Json $jsonAzure "src\Config.Net.Azure\project.json"
Set-Json $jsonTests "src\Config.Net.Tests\project.json"

