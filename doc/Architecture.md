On top of the class hierarchy is the `IConfigManager` class. It exposes a simplistic interface to read and write settings:

```csharp
   /// <summary>
   /// Top interface to read and write configuration values in your application or a part of it.
   /// </summary>
   public interface IConfigManager
   {
      /// <summary>
      /// Reads the value by key
      /// </summary>
      Property<T> Read<T>(Setting<T> key);

      /// <summary>
      /// Reads the nullable value by key
      /// </summary>
      Property<T?> Read<T>(Setting<T?> key) where T : struct;

      /// <summary>
      /// Writes a value by key
      /// </summary>
      void Write<T>(Setting<T> key, T value);

      /// <summary>
      /// Writes a nullable value by key
      /// </summary>
      void Write<T>(Setting<T?> key, T? value) where T : struct;
   }
```
`Read` and `Write` methods are duplicated to support nullable type parameters, however internally they serve the same purpose.

This interface is the entry point to configuration calls you will make from your application.

You are probably already using the global `Cfg` class - it simply redirects the calls to a built-in IConfigManager which is pre instantiated for you. 

## IConfigManagerConfig

The global `Cfg` class exposes `IConfigManagerConfig` which serves the only purpose - to configure the configuration. It contains operations such as adding or removing stores, and a few others described later.

## The type system

All types in Config.Net are strong. At the moment only the following types are supported natively:

* enumerations
* `bool`
* `double`
* `int`
* `long`
* `string`
* `string[]`
* `DateTime`
* `TimeSpan`
* `JiraTime`

All the supported types can be nullable.

# Type representation

Config.Net writes and reads strings to the configuration stores. This decision was made to let the custom store implementers do it quick and easy as they have to only implement one Read and one Write method which saves or loads a plain string.

Internally each type is handled by a class which implements `ITypeParser<T>`:

```csharp
   public interface ITypeParser<T> : ITypeParser
   {
      bool TryParse(string value, out T result);

      /// <summary>
      /// Converts T to raw string value
      /// </summary>
      /// <param name="value">T value</param>
      /// <returns>Raw string for value of type T</returns>
      string ToRawString(T value);
```

System.Enum is an exception from this rule as it doesn't play well with generic types. Enum handling is implemented as a hack inside the Config.Net core, but it works well.

Config.Net expects types to be represented in a specific format. If you set the configuration values manually you should know which format is expected, which is detailed below.

## string

Strings are just strings, no special requirements.

## string[]

String arrays are stored as comma (,) or space ( ) separated values. For example in an INI file you would write:

```
Cities=London,NewYork
```

Note that you cannot put space in NewYork value as it will be treate as two values instead of one.

## bool

Boolean representation is flexible. Config.Net considers the following values as true:

* true
* yes
* 1

and false:

* false
* no
* 0

regardless of the case.

## int, long, double

These are stored as you would normally write them down in code or on paper. Standard .NET conversion functions are used to read and write the values.

## DateTime

Dates are stored in [Universal Sortable Format](https://msdn.microsoft.com/en-us/library/az4se3k1(v=vs.110).aspx#UniversalSortable).

## TimeSpan

TimeSpan is already human readable if you try to convert it to string in .NET, therefore the native format is used.

## JiraTime

Inspired by Jira time etimation fields this format is more human readable. You can specify time length using the following time notation:

* d - day
* h - hour
* m - minutes
* s - seconds
* ms - milliseconds

Examples:
* 1d1h2m3s - 1 day 1 hour 2 minutes and 3 seconds
* 5m3ms - 5 minutes 3 milliseconds

## Extending the type system

You are welcome to add more supported types. In order to do that implement the `ITypeParser<T>` interface where T is your custom type. Then register the type with a call to `Cfg.Configuration.RegisterParser()`. If you think the type is useful for others please don't forget to contribute to the core source code.

## The Property<T>

You may have already noticed that `Read` methods returns `Property<T>`, however you are casting it to the actual strong type and it just works. This is because `Property<T>` has an implicit casting operator:

```csharp
public static implicit operator T(Property<T> property)
{
   return property.Value;
}
```

So what's the point of returning `Property<T>` instead of the actual value? Internally Config.Net operates with this generic type instead of actual values for a several reasons:

* it contains some internal helpers methods to work with the values
* this class is just a definition of a property value but the actual value may change over time. Some stores like `IniFileStore` may react on file change on the file system and automatically re-read the values. In this case if your code uses `Property<T>` internally instead of the actual raw value it will receive the new value immediately.
* `Property<T>` has a useful event `ValueChanged` for the situations like above.

# Caching

There is no caching implemented in the current version. Meaning that each call to `Cfg.Read` results in a `Read` call to a configured store.

Caching is on the roadmap though so come back later.
