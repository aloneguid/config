# JSON File Store

JSON is supported in **read-only mode** and is using `System.Text.Json` namespace. For this reason it's only available in projects targeting .NET Core 3, .NET 5 and .NET 6.

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
