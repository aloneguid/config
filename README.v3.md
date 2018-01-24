# Config.Net [![Build status](https://ci.appveyor.com/api/projects/status/f89ye0oldduk9bwi?svg=true)](https://ci.appveyor.com/project/aloneguid/config) [![NuGet](https://img.shields.io/nuget/v/Config.Net.svg?maxAge=2592000?style=flat-square)](https://www.nuget.org/packages/Config.Net/)

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
    public Option<string> AuthClientId {get; set;}

    public readonly Option<string> AuthClientSecret;

    protected override void OnConfigure(IConfigConfiguration configuration)
    {
         configuration.UseAppConfig();
    }
}
```

The values stored in the configuration all inherit from an 'Option', and are strongly typed using the format `Option<T>` where `T` is any standard base type.

The publically accessible fields can be defined as either Properties or Fields. The advantage of a Property is that it allows definition of a interfaces for decoupling and IOC injection, but Fields are supported for backwards compatibility.

Let's go through this code snippet:
* We have declared `AllSettings` class which will store configuration for oru application. All configuration classes must derive from `SettingsContainer`.
* Two strong-typed configuration options were declared. Note they can be either properties or fields, and both are `readonly` which is another plus towards code quality.
* `Option<T>` is a configuration option definition in Config.Net where generic parameter specifies the type. There is a limited set of [supported types](doc/SupportedTypes.md) and you can [create your own](doc/CustomParsers.md)
* `OnConfigure` method implementation specifies that app.config should be used as a configuration store.

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


### Using Multiple Sources

`OnConfigure` method is used to prepare settings container for use. You can use it to add multiple configuration sources. To get the list of sources use IntelliSense (type dot-Use after `configuration`). For instance this method implementation:

```csharp
protected override void OnConfigure(IConfigConfiguration configuration)
{
    configuration.UseAppConfig();
    configuration.UseEnvironmentVariables();
}

```

causes the container to use both app.config and environment variables as configuration source.

The order in which sources are added is important - Config.Net will try to read the source in the configured order and return the value from the first store where it exists.

### Writing Settings

Some configuration stores support writing values. You can write the value back by calling the `.Write()` method on an option definition:

```csharp
c.AuthClientId.Write("new value");
```

Config.Net will write the value to the first store which supports writing. If none of the stores support writing the call will be ignored.


## Caching

By defalut config.net caches configuration values for 1 hour. After that it will read it again from the list of configured stores. If you want to change it to something else set the variable in the `OnConfigure` method:

```csharp
protected override void OnConfigure(IConfigConfiguration configuration)
{
    configuration.CacheTimeout = TimeSpan.FromMinutes(1);	//sets caching to 1 minute

    configuration.UseAppConfig();
    configuration.UseEnvironmentVariables();
}
```

setting it to `TimeSpan.Zero` disables caching completely.

# Ways to declare settings

There are a few requirements to declaring a setting a s member of derived `SettingsContainer` class:

* If a property is used, it must be either read-write if no default is supplied, or can be read-only if a default value is provided (recommended)
* If a field is used, it must be marked as read-only, and cannot be static

A setting has a few basic properties:

* **Name** which by default is the same as a variable name. Config.Net uses reflection to get the variable name. Variable name is important as it's used to read the option from a specific store.
* **Default Value** which is defaulted to the default value of the type i.e. `0` for `int`, `null` for classes etc.

The simplest form of declaring an option is:

```csharp
public Option<string> AuthClientId  {get; set;}
```
or
```csharp
public readonly Option<string> AuthClientId = new Option<string>();
```

This sets option name to `AuthClientId` and default value to `null`. However if you need to specify the name explicitly change it to the following:

```csharp
public Option<string> AuthClientId {get; } = new Option<string>("AuthenticationClientId", null);
```
or
```csharp
public readonly Option<string> AuthClientId = new Option<string>("AuthenticationClientId", null);
```

This sets option name to `AuthenticationClientId` whereas local variable name is still `AuthClientId`. It is recommended that you set option name explicitly anyway, even if it matches the variable name. It saves from potential refactoring problems as when you rename the variable but config files still hold the old name.

## Default Value

Default value is returned in case an option cannot be found in any of the stores. This is useful in situations when you want to introduce a configurable option, but don't want to store it in an external configuration store yet. In fact you can use options to declare constants in your code that way.

To change the default value pass it as a constructor argument in option initialisation:

```csharp
public Option<string> AuthClientId  {get; } = new Option<string>("AuthenticationClientId", "default id");
```

This always returns `"default id"` when `AuthenticationClientId` is not found in any of the configured stores.

# Available Stores

The list of available built-in and external stores is maintained on this page:


| Name                 | Readable | Writeable | Package  | Purpose                  |
|----------------------|----------|-----------|----------|--------------------------|
|[AppConfig](doc/Stores_AppConfig.md)|v|x|internal|.NET app.config files|
|[AppConfigSection](doc/Stores_AppConfig.md)|v|x|internal|.NET app.config files (custom sections)|
|[EnvironmentVariables](doc/Stores_EnvironmentVariables.md) | v        | v         | internal | OS environment variables |
|[CommandLine](doc/Stores_CommandLine.md)|v|x|internal|Parsing command line as configuration source| 
|[IniFile](doc/Stores_IniFile.md)              | v        | v         | internal | INI files |
|[JSON](doc/Stores_JsonFile.md)|v|v|internal|JSON file storage|
|[InMemory](doc/Stores_InMemory.md)             | v        | v         | internal | In-memory storage |
|[Storage.Net Integration](doc/Stores_StorageNet.md)|v|v|external|anything that [storage.net](https://github.com/aloneguid/storage) supports


Proudly using [Jetbrains Rider](https://www.jetbrains.com/rider/) for Open Source Development.

![Jetbrains Rider](doc/jetbrains_rider_small.png)