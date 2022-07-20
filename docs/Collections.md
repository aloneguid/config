# Collections

Config.Net supports collections for primitive types and **interfaces**. 

Collection must be always declared as `IEnumerable<T>` and only have a getter.

At the moment collections are read-only and writing to collections may be supported at some point in future releases.

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

Collections at the moment are only supported in read-only mode. All of the stores do support collections, however due to the nature of some underlying implementations complex collections are harder to represent and they are following the [flat line syntax](flatline.md).