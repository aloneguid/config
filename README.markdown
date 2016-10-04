# Config.Net ![](https://aloneguid.visualstudio.com/DefaultCollection/_apis/public/build/definitions/323c5f4c-c814-452d-9eaf-1006c83fd44c/4/badge) [![Gitter](https://badges.gitter.im/aloneguid/config.svg)](https://gitter.im/aloneguid/config?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge) [![NuGet](https://img.shields.io/nuget/v/Config.Net.svg?maxAge=2592000?style=flat-square)](https://www.nuget.org/packages/Config.Net/)

A comprehensive easy to use and powerful .NET configuration library, fully covered with unit tests and tested in the wild on thousands of servers and applications.

This library eliminates the problem of having configuration in different places, having to convert types between different providers, hardcoding configuration keys accross the solution, depending on specific configuration source implementation. It's doing that by exposing an abstract configuration interface and providing most common implementation for configuration sources like app.config, environment variables etc.

## Quick Start

Usually developers will hardcode reading cofiguration values from different sources like app.config, local json file etc. For instance, consider this code example:

```csharp
var clientId = ConfigurationManager.AppSettings["AuthClientId"];
var clientSecret = ConfigurationManager.AppSettings["AuthClientSecret"];

```

You would guess that this code is trying to read a configuration setting from the local app.config file by name and that might be true, however there are numerous problems with this approach:

* settings are referenced by a hardcoded string name which is prone to typos and therefore crashes in runtime.
* there is no easy way to find out where a particular setting is used in code, except for performing a fulltext search (provided that the string was not mistyped)
* if you decide to store configuration in a different place the code must be rewritten.

Welcome to Config.Net which solves most of those problems. Let's rewrite this abomination using Config.Net approach. First, we need to define a configuration container which describes which settings are used in your application or a library:


### Declare Settings Container

```csharp
using Config.Net;

public class AllSettings : SettingsContainer
{
    public readonly Option<string> AuthClientId;

    public readonly Option<string> AuthClientSecret;

    protected override void OnConfigure(IConfigConfiguration configuration)
    {
         configuration.UseAppConfig();
    }
}
```

Let's go through this code snippet:
* We have declared `AllSettings` class which will store configuration for oru application. All configuration classes must derive from `SettingsContainer`.
* Two strong-typed configuration options were declared. Note they are both `readonly` which is another plus towards code quality.
* `Option<T>` is a configuration option definition in Config.Net where generic parameter specifies the type.
* `OnConfigure` mehtod implementation specifies that app.config should be used as a configuration store.

### Use Settings

Once container has been defined start using the settings, for instance:

```csharp
var c = new AllSettings();

string clientId = c.AuthClientId;
string clientSecret = c.AuthClientSecret;
```

Two things worth to note in this snippet:
* An instance of `AllSettings` container was created. Normally you would create an instance of a settings container per application instance for performance reasons.
* The settings were read from the settings container. Note the syntax and that for example `AuthClientId` is defined as an `Option<string>` but casted to `string`. This is because `Option<T>` class has implicit casting operator to `T` defined.

### Writing Settings

todo

### 


--- old docs below ---


## Caching

By defalut config.net caches configuration values for 1 hour. After that it will read it again from the list of configured stores. If you want to change it to something else set the following variable:

```csharp
Cfg.Configuration.CacheTimeout = TimeSpan.FromHours(1);
```

setting it to `TimeSpan.Zero` disables caching completely.

## Best practices for declaring settings

Usually you would declare settings used in your application either in the application itself or a shared library in a file like Settings.cs:

```csharp
   static class Settings
   {
      public static readonly Setting<string> AzureStorageName = new Setting<string>("Azure.Storage.Name", null);

      public static readonly Setting<string> AzureStorageKey = new Setting<string>("Azure.Storage.Key", null);
   }
```

It's recommended to declared them as `public static readonly` fields.

# Available Stores

## App.Config

[AppConfigStore](https://github.com/aloneguid/config/blob/master/src/Config.Net/Stores/AppConfigStore.cs) simply reads keys from the default ConfigurationManager and has one parameterless constructor.

## Assembly Config

[AssemblyConfigStore](https://github.com/aloneguid/config/blob/master/src/Config.Net/Stores/AssemblyConfigStore.cs) reads those .dll.config files rarely used by anyone. You need to pass `Assembly` reference to read from.

## System Environment variables

[EnvironmentVariablesStore](https://github.com/aloneguid/config/blob/master/src/Config.Net/Stores/EnvironmentVariablesStore.cs) operates on system environment variables. Reads and writes are supported.

## INI Files

[IniFileConfigStore](https://github.com/aloneguid/config/blob/master/src/Config.Net/Stores/IniFileConfigStore.cs) works with INI files. Both reads and writes are supported. INI sections are supported too. This store will treat the first dot in the key name as a section separator, for example if you key is defined as `Azure.StorageKey` this store will expect to see the following in the INI file:

```
[Azure]
StorageKey=value
```

Both multiline and single line comments are preserved when writing to the file.

## In-Memory configuration

[InMemoryConfigStore](https://github.com/aloneguid/config/blob/master/src/Config.Net/Stores/InMemoryConfigStore.cs) simply stores configuration in memory.

## Microsoft Azure Configuration

[AzureConfigStore](https://github.com/aloneguid/config/blob/master/src/Config.Net.Azure/AzureConfigStore.cs)  is a simple wrapper around [CloudConfigurationManager](https://msdn.microsoft.com/en-us/library/azure/mt634650.aspx). It allows you to read configuration settings from Azure Websites, Cloud Services and other Microsoft Azure PaaS services which are compatible with this class.

In addition to that another implementation [AzureTableConfigStore](https://github.com/aloneguid/config/blob/master/src/Config.Net.Azure/AzureTableConfigStore.cs) allows to read/write configuration from a table in Azure Storage which is the cheapest storage machanism in Azure world. in order to use it you have to initialise it with storage account name, give it storage key and desired table name. The resulting table in the storage will look like this:

| Partition Key |     Row Key    |  Value    |
|---------------|----------------|-----------|
|  App name     |  config key    | key value |

