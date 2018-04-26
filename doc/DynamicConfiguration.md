# Dynamic Configuration

Sometimes having just setters and getters is not enough or you need to get a configuration key by name you know only at runtime. That's where dynamic configuration comes in place.

With dynamic configuration you can declare methods in configuration interface, not just properties. Have a look at this example:

```csharp
public interface ICallableConfig
{
   string GetName(string keyName);
}
```

Calling the method will make Config.Net to read configuration using with key set to **Name**.*value of `keyName`*. For instance calling this method as `.GetName("my_key")` will return a value by key `Name.my_key`.

The first part of the key comes from the method name itself (any `Get` or `Set` method prefixes are removed automatically). The `[Option]` attribute applies here if you'd like to customise the key name i.e.

```csharp
public interface ICallableConfig
{
   [Option(Alias = "CustomName")]
   string GetName(string keyName);
}
```

changes the key to **CustomName**.*value of `keyName`*.

Please take a special note that if you call a method just `Get(string name)` Config.Net will read settings from a root namespace i.e. `Get("myprop")` will return a value by key `myprop`. Essentially this allows you to *read from the store dynamically*, however you are losing the ability of performing type safe conversions.

## Multiple Parameters

You can declare a method with as many parameters as you want, they will be simply chained together when deciding which key name to use, for example:


```csharp
public interface ICallableConfig
{
   string GetName(string keyName, string subKeyName);
}
```

will read configuration with key **Name**.*value of `keyName`*.*value of `subKeyName`* etc.

## Writing Values

The same way as you declare a method to read values, you can also declare methods for writing values. The only difference is that a method which writes values *must be void*. The last parameter of a writing method is considered a value parameter, for example:

```csharp
public interface ICallableConfig
{
   void SetName(string keyName, string value);
}
```