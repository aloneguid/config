# Config.Net

![](https://aloneguid.visualstudio.com/DefaultCollection/_apis/public/build/definitions/323c5f4c-c814-452d-9eaf-1006c83fd44c/4/badge)

The library is available as a [NuGet package](https://www.nuget.org/packages/Config.Net)

A comprehensive easy to use and powerful .NET configuration library, fully covered with unit tests and tested in the wild on thousands of servers and applications.

This library eliminates the problem of having configuration in different places, having to convert types between different providers, hardcoding configuration keys accross the solution etc.

## Configuration sources

Config.Net supports the following configuration sources out of the box:

* Standard .NET app.config file
* Standard .NET assembly config (.dll.config)
* INI files
* System environment variables
* In memory configuration store
* Microsoft Azure Configuration source (Web Apps, API Apps, Cloud Services etc.) - available as a separate [NuGet Package](https://www.nuget.org/packages/Config.Net.Azure)

## Quick Start

All configuration settings are strong typed and the library takes care of making sure types are converted, stored and retreived properly.

Each configuration setting is described by a strong typed `Setting<T>` class. Let's assume your app needs to store an integer value (many types are supported), then you would normally create an AppSettings.cs class shared between the modules which has the following definition:

### Add Setting definitions

```csharp
public static class AppSettings
{
  public static readonly Setting<int> MyIntegerSetting = new Setting<int>("Namespace.App.MyIntegerSetting", 5);

  //more setting definitions
}
```

This definition has a few important points.

* It has a type of `int` as `Setting<T>` is strong typed.
* The first constructor parameter sets the setting name which has to be unique. This key is used in a store implementation to save and retreive the value.
* Second constructor parameter is a default value. It's strongly typed and in this case is _int_. Default values are very  useful. If your store does not contain a setting with specified key or you simply don't have any stores configured the library will return the default value. In addition to that if you try to save default value to a setting it will be deleted on the target store to save spece, but more about this later.

### Configure configuration source(s)

In order for application to know where the configuration is stored you need to configure one or more stores on app initialisation. If you don't add any of the stores the library will still work as expected, however only the default values will be returned (still very useful in many cases). In fact you may want some values not to be in a store at all, and use the setting definition to almost define constants which you can potentially change in future at any time by setting the value in one of the stores and magic happens - application uses a changed value!

As an example, if your settings are stored in the standard app.config file add this on application init:

```csharp
Cfg.Configuration.AddStore(new AppConfigStore());
```

### Read the value from code

The easiest way to get the value is call to the definition itself:

```csharp
int value = AppSettings.MyIntegerSetting;
```

Under the hood config.net calls to default configuration manager and tries to read the value from the list of configuration stores. The first store that returns the value will be used as the result, otherwise default value is returned (in your case it's `5`).

## Caching

By defalut config.net caches configuration values for 1 hour. After that it will read it again from the list of configured stores. If you want to change it to something else set the following variable:

```csharp
Cfg.Configuration.CacheTimeout = TimeSpan.FromHours(1);
```

setting it to `TimeSpan.Zero` disables caching completely.

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

