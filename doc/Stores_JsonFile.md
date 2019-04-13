# JSON File Store

Since v4.5 JSON store provider is located in a separate package [![NuGet](https://img.shields.io/nuget/v/Config.Net.Json.svg)](https://www.nuget.org/packages/Config.Net.Json)

## Configuring

### Mapping to file

```csharp
IMySettings settings = new ConfigurationBuilder<IMySettings>()
   .UseJsonFile(path)
   .Build();
```

This variant supports reading and writing. `Path` can be either relative or absolute.

### Mapping to file contents

```csharp
IMySettings settings = new ConfigurationBuilder<IMySettings>()
   .UseJsonString(path)
   .Build();
```

This variant supports reading only as there is nowhere to write in this case.

### Mapping to appsettings.json file

```csharp
IMySettings settings = new ConfigurationBuilder<IMySettings>()
   .UseJsonConfig()
   .Build();
```

This variant supports reading only. Requires "appsettings.json" file in root.

##### Environment
`appsettings.json` can have multiple environment files like `appsettings.Debug.json` and `appsettings.Staging.json` etc.
 The builder will search for "APP_ENV" environment variable and override `appsettings.json` partially or fully according to environment file. If environment file `appsettings.{Environment}.json` is missing, no exception will be raised and  only `appsettings.json` will be used.

This method is similar to [Use multiple environments in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/environments?view=aspnetcore-2.2).
The advantage of this approach on other configuration transformation methods is not having to build the code in order to 
get the environment configuration.

##### Advanced usage

```csharp
IMySettings settings = new ConfigurationBuilder<IMySettings>()
   .UseJsonConfig("mysettings.json", settings =>
      {
         settings.MergeArrayHandling = MergeArrayHandling.Merge;
         settings.MergeNullValueHandling = MergeNullValueHandling.Ignore;
      })
   .Build();
```

This variant supports reading only. `Path` can be either relative or absolute.

See [Merging JSON](https://www.newtonsoft.com/json/help/html/MergeJson.htm) for other `settings` options.

## Using

In the simplest form every key in the JSON file corresponds to the name of an option. For instance a definition

```csharp

public interface IMySettings
{
   string AuthClientId { get; }
   string AuthClientSecreat { get; }
}
```

will correspond to the following JSON file:

``` json
{
   "AuthClientId":"Id",
   "AuthClientSecret":"Secret"
}
```

## Using a setting that has a non trivial JSON Path
In a more advanced, and probably more typical scenario, the JSON setting will be nested within the configuration structure in a non trivial way (i.e., not on the root with an identical name). The Option attribute, combined with Alias property, specifies the JSON Path needed in order to reach the setting's value.

```csharp

public interface IMySettings
{
   string AuthClientId { get; }
   string AuthClientSecreat { get; }
   
   [Option(Alias = "WebService.Host")]
   string ExternalWebServiceHost { get; }
}
```

will correspond to the following JSON file:

``` json
{
   "AuthClientId":"Id",
   "AuthClientSecret":"Secret",
   
   "WebService": {
       "Host": "http://blahblah.com:3000"
   }
}
```
