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

```csharp
public static class AppSettings
{
  public static readonly Setting<int> MyIntegerSetting = new Setting<int>("Namespace.App.MyIntegerSetting", 5);

  //more setting definitions
}
```

This definition has a few important points.

* It has a type of _int_ as Setting<T> is strong typed.
* The first constructor parameter sets the setting name which has to be unique. This key is used in a store implementation
  to save and retreive the value.
* Second constructor parameter is a default value. It's strongly typed and in this case is _int_. Default values are very
  useful. If your store does not contain a setting with specified key or you simply don't have any stores configured
  the library will return the default value. In addition to that if you try to save default value to a setting it will be
  deleted on the target store to save spece, but more about this later.

### Configuration Stores

In order for application to know where the configuration is stored you need to configure one or more stores on app
initialisation. If you don't add any of the stores the library will still work as expected, however only the default
values will be returned (still very useful in many cases). In fact you may want some values not to be in a store at all,
and use the setting definition to almost define constants which you can potentially change in future at any time
by setting the value in one of the stores and magic happens - application uses a changed value!

As an example, if your settings are stored in the standard app.config file add this on application init:

```csharp
Cfg.Configuration.AddStore(new AppConfigStore());
```

The full list of available stores it [here](https://github.com/aloneguid/config/wiki/List-of-configuraton-stores)

Note that you can add or remove configuration stores at any point of the application lifecycle. The only thing you have
to keep in mind is the library will be able to read the stores only after adding them, otherwise default values will be
returned.

You can add more than one store to the configuration, for example if I store some of the values in app.config and others in
an .INI file on disk I can do the following:

```csharp
Cfg.Configuration.AddStore(new AppConfigStore());
Cfg.Configuration.AddStore(new IniFileConfigStore("c:\\settings.ini"));
```

Note that Config.Net will try to read the app.config first, and if the value is not found - try the ini file. I.e. reading
is performed in the order of adding the stores to configuration.

### Reading configuration

Reading configuration is much easier than you would expect. Config.Net has a public static Cfg class with a few important
methods:

```csharp
public static Property<T> Read<T>(Setting<T> key);
public static Property<T?> Read<T>(Setting<T?> key) where T : struct;
```

So if you'd like to read the setting you've declared previously you would normally write the following:

```csharp
int value = Cfg.Read(AppSettings.MyIntegerSetting);
```

Read method returns Property<T> or (<T?> for nullable types) however an implicit conversion operator gives you back
the type you have declared.

You might also think this is a bad architectural approach to use global static class for those, but you will be wrong - 
this is only a helper class to make things as easy as possible. Therefore if you need to be more flexible, or use
Config.Net in mocking and unit testing, with IoC containers and all that beautiful stuff read the page on [Architecture](https://github.com/aloneguid/config/wiki/Architecture).

## Best Practices

Config.Net makes configuration extremely easy and it's great, however it is also a big problem. Developers tend to
cut the corners as much as they can (at least the ones I know) and generate dirty unrefactorable code in minutes.

Try not to overuse the simplicity and do not use Config.Net everywhere in the code as a cross cutting concern. Use it
merely for initialisation of your classes, not inside the business logic. Create overloaded constructors which
allow you to pass the dependent values as well as read them from Cfg class. Or in the worst case pass IConfigurationManager - 
read more on this in the [architecture](https://github.com/aloneguid/config/wiki/Architecture) section.

Keep your classes unit testable and IoC enabled by design.

## Roadmap

* Active Directory configuration
* Android configuration store (done, needs migrating)
* Windows Phone configuration store (done, needs migrating)
