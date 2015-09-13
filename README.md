# Config.Net

The library is available as a [NuGet package](https://www.nuget.org/packages/Config.Net)

A comprehensive easy to use and powerful .NET configuration library, fully covered with unit tests and tested in
the wild on thousands of servers and applications.

Configuration is an interesting and complicated area in .NET and other platforms. This library eliminates the
problem of having configuration in different places, having to convert types between different providers, hardcoding
configuration keys accross the solution etc.

## Configuration sources

Config.Net supports the following configuration sources out of the box:

* Standard .NET app.config file
* Standard .NET assembly config (.dll.config)
* INI files
* In memory configuration store
* ... and more coming, please suggest

Config.Net comes with optional support for Azure storage as well:

* Windows Azure Configuration Manager
* Windows Azure Table Storage can be used to store configuration as well


## Extensibility

Config.Net is extremely easy to extend to support more providers, by implementing only one method which can retreive a
string value by string key. Optionally, if your storage supports write access you need to implement write method
which accepts string key and string value.

Config.Net takes care of converting types and enforcing strong typing by itself.

## Philosophy

Config.Net philosophy is to make configuration as easy as possible. All configuration settings are strong typed and
the library takes care of making sure types are converted, stored and retreived properly.

Each configuration setting is described by a strong typed `Setting<T>` class. Let's assume your app needs to store an
integer value (many types are supported), then you would normally create an AppSettings.cs class shared between the modules
which has the following definition:

### Setting definition

```public static class AppSettings
{
  public static readonly Setting<int> MyIntegerSetting = new Setting<int>("Namespace.App.MyIntegerSetting", 5);

  //more setting definitions
}```

This definition has a few important points.

* It has a type of _int_ as Setting<T> is strong typed.
* The first constructor parameter sets the setting name which has to be unique. This key is used in a store implementation
  to save and retreive the value.
* Second constructor parameter is a default value. It's strongly typed and in this case is _int_. Default values are very
  useful. If your store does not contain a setting with specified key or you simply don't have any stores configured
  the library will return the default value. In addition to that if you try to save default value to a setting it will be
  deleted on the target store to save spece, but more about this later.

### Configuration Stores

_todo_

## Best Practices

_todo_

## Roadmap

* Active Directory configuration
* Android configuration store (done, needs migrating)
* Windows Phone configuration store (done, needs migrating)