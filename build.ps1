param(
   [switch]
   $Publish,

   [string]
   $NuGetApiKey
)

$VersionPrefix = "3"
$VersionSuffix = "1.2.0"
$Copyright = "Copyright (c) 2015-2017 by Ivan Gavryliuk"
$PackageIconUrl = "http://i.isolineltd.com/nuget/config.net.png"
$PackageProjectUrl = "https://github.com/aloneguid/config"
$RepositoryUrl = "https://github.com/aloneguid/config"
$Authors = "Ivan Gavryliuk (@aloneguid)"
$PackageLicenseUrl = "https://github.com/aloneguid/config/blob/master/LICENSE"
$RepositoryType = "GitHub"

$SlnPath = "src\Config.Net.sln"
$AssemblyVersion = "$VersionPrefix.0.0.0"
$PackageVersion = "$VersionPrefix.$VersionSuffix"
Write-Host "version: $PackageVersion, assembly version: $AssemblyVersion"

function Set-VstsBuildNumber($BuildNumber)
{
   Write-Verbose -Verbose "##vso[build.updatebuildnumber]$BuildNumber"
}

function Update-ProjectVersion([string]$Path)
{
   $xml = [xml](Get-Content $Path)

   if($xml.Project.PropertyGroup.Count -eq $null)
   {
      $pg = $xml.Project.PropertyGroup
   }
   else
   {
      $pg = $xml.Project.PropertyGroup[0]
   }

   $pg.Version = $PackageVersion
   $pg.FileVersion = $PackageVersion
   $pg.AssemblyVersion = $AssemblyVersion
   $pg.Copyright = $Copyright
   $pg.PackageIconUrl = $PackageIconUrl
   $pg.PackageProjectUrl = $PackageProjectUrl
   $pg.RepositoryUrl = $RepositoryUrl
   $pg.Authors = $Authors
   $pg.PackageLicenseUrl = $PackageLicenseUrl
   $pg.RepositoryType = $RepositoryType

   $xml.Save($Path)
}

function Exec($Command)
{
   Invoke-Expression $Command
   if($LASTEXITCODE -ne 0)
   {
      Write-Error "command failed (error code: $LASTEXITCODE)"
      exit 1
   }
}

# General validation
if($Publish -and (-not $NuGetApiKey))
{
   Write-Error "Please specify nuget key to publish"
   exit 1
}

# Update versioning information
Get-ChildItem *.csproj -Recurse | Where-Object {-not($_.Name -like "*test*")} | % {
   $path = $_.FullName
   Write-Host "setting version on $path"
   Update-ProjectVersion $path
}
Set-VstsBuildNumber $Version

# Restore packages
Exec "dotnet restore $SlnPath"

# Build solution
Get-ChildItem *.nupkg -Recurse | Remove-Item -ErrorAction Ignore
Exec "dotnet build $SlnPath -c release"

# publish the nugets
if($Publish.IsPresent)
{
   Write-Host "publishing nugets..."

   Get-ChildItem *.nupkg -Recurse | % {
      $path = $_.FullName
      Write-Host "publishing from $path"

      Exec "nuget push $path -Source https://www.nuget.org/api/v2/package -ApiKey $NuGetApiKey"
   }
}

Write-Host "build succeeded."