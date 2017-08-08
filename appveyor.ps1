$gv = $env:APPVEYOR_BUILD_VERSION
if($gv -eq $null)
{
   $gv = "3.2.0"
}
$vt = @{
   "Config.Net.Integration.Storage.Net.csproj" = "1.0.$gv";
}

Write-Host "global version is $gv"

$Copyright = "Copyright (c) 2015-2017 by Ivan Gavryliuk"
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

   $v = $vt.($File.Name)
   if($v -eq $null) { $v = $gv }

   $xml = [xml](Get-Content $File.FullName)

   if($xml.Project.PropertyGroup.Count -eq $null)
   {
      $pg = $xml.Project.PropertyGroup
   }
   else
   {
      $pg = $xml.Project.PropertyGroup[0]
   }

   $parts = $v -split "\."
   $bv = $parts[2]
   if($bv.Contains("-")) { $bv = $bv.Substring(0, $bv.IndexOf("-"))}
   [string] $fv = "{0}.{1}.{2}.0" -f $parts[0], $parts[1], $bv
   [string] $av = "{0}.0.0.0" -f $parts[0]
   [string] $pv = $v

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

Write-Host "build succeeded."