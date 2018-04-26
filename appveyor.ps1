$BuildNo = $env:APPVEYOR_BUILD_NUMBER
$Major = 4
$Minor = 9
$Patch = 0
$IsPrerelease = $false

# latest release - 4.9.0

if($BuildNo -eq $null)
{
   $BuildNo = "1"
}

$Copyright = "Copyright (c) 2015-2018 by Ivan Gavryliuk"
$PackageIconUrl = "http://i.isolineltd.com/nuget/config.net.png"
$PackageProjectUrl = "https://github.com/aloneguid/config"
$RepositoryUrl = "https://github.com/aloneguid/config"
$Authors = "Ivan Gavryliuk (@aloneguid)"
$PackageLicenseUrl = "https://github.com/aloneguid/config/blob/master/LICENSE"
$RepositoryType = "GitHub"

$SlnPath = "src\Config.Net.sln"

function Update-ProjectVersion($File)
{
   Write-Host "updating $File ..."

   $xml = [xml](Get-Content $File.FullName)

   if($xml.Project.PropertyGroup.Count -eq $null)
   {
      $pg = $xml.Project.PropertyGroup
   }
   else
   {
      $pg = $xml.Project.PropertyGroup[0]
   }

   if($IsPrerelease) {
      $suffix = "-ci-" + $BuildNo.PadLeft(5, '0')
   } else {
      $suffix = ""
   }

   
   [string] $fv = "{0}.{1}.{2}.{3}" -f $Major, $Minor, $Patch, $BuildNo
   [string] $av = "{0}.0.0.0" -f $Major
   [string] $pv = "{0}.{1}.{2}{3}" -f $Major, $Minor, $Patch, $suffix

   $pg.Version = $pv
   $pg.FileVersion = $fv
   $pg.AssemblyVersion = $av

   Write-Host "$($File.Name) => fv: $fv, av: $av, pkg: $pv"

   $pg.Copyright = $Copyright
   $pg.PackageIconUrl = $PackageIconUrl
   $pg.PackageProjectUrl = $PackageProjectUrl
   $pg.RepositoryUrl = $RepositoryUrl
   $pg.Authors = $Authors
   $pg.PackageLicenseUrl = $PackageLicenseUrl
   $pg.RepositoryType = $RepositoryType

   $xml.Save($File.FullName)
}

function Exec($Command, [switch]$ContinueOnError)
{
   Invoke-Expression $Command
   if($LASTEXITCODE -ne 0)
   {
      Write-Error "command failed (error code: $LASTEXITCODE)"

      if(-not $ContinueOnError.IsPresent)
      {
         exit 1
      }
   }
}

# Update versioning information
Get-ChildItem *.csproj -Recurse | Where-Object {-not(($_.Name -like "*test*") -or ($_.Name -like "*runner*"))} | % {
   Update-ProjectVersion $_
}

# Restore packages
Exec "dotnet restore $SlnPath"

function Get-DisplayVersion()
{
   $v = "$Major.$Minor.$Patch"
   
   if($IsPrerelease)
   {
      $v = "$v-ci-$BuildNo"
   }

   $v
}

$VDisplay = Get-DisplayVersion
Invoke-Expression "appveyor UpdateBuild -Version $VDisplay"