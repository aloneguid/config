# Config.Net

_This library is in process of migration from private solution to opensource and is not ready to use yet. Please come back later_

The library is available as a [NuGet package](https://www.nuget.org/packages/Config.Net)

A comprehensive easy to use and powerful .NET configuration library.
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