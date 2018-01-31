# Collections

Config.Net supports collections for primitive types and interfaces. 

Collection must be always declared as `IEnumerable<T>`.

## Primitive Types
Suppose you want to read an array of integers, this can be declared as:

```csharp
interface IMyConfig
{
   IEnumerable<int> Numbers { get; }
}
```

## Interfaces
Reading an array of primitive types is not that interesting as you can always implement parsing yourself by storing some kind of a delimiter in a string i.e. `1,2,3,4` etc. Config.Net allows you to read a collection of your own complex types like so:

```csharp

interface ICredentials
{
   string Username { get; }
   string Password { get; }
}

interface IMyConfig
{
   IEnumerable<ICredentials> AllCredentials { get; }
}
```

## Limitations

At the moment this feature is relatively new and not every store supports it.