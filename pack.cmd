msbuild src/Config.Net.sln /p:Configuration=Release
nuget pack config.net.nuspec
nuget pack config.net.azure.nuspec