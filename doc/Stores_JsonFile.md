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