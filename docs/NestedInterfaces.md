# Nested Interfaces

Interfaces can be nested into each other. This is useful when you have a different set of similar settings and you don't want to redeclare them, for example let's say we want to store normal and admin credentials in a configuration storage.

First, we can declare an interface to store credentials in general:

```csharp
public interface ICreds
{
   string Username { get; }

   string Password { get; }
}
```

and contain it withing our configuration interface:

```csharp
public interface IConfig
{
   ICreds Admin { get; }

   ICreds Normal { get; }
}
```

then, instantiate `IConfig`:

```csharp
_config = new ConfigurationBuilder<IConfig>()
   .Use...
   .Build();
```

Now you can get credentials with a normal C# syntax, for instance to get admin username `_config.Admin.Username` etc. 

All the attributes are still applicable to nested interfaces.

When getting property values, each nesting level will be separated by a dot (`.`) for instance admin username is fetched by key `Admin.Username` - something to keep in mind when using flat configuration stores.