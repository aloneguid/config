# Creating Custom Type Parsers

In cases `Config.Net` does not support data type you work with there are two recommended ways to fix it:

- If this type is widely used i.e. part of .NET, contribute to `Config.Net` source code.
- If this is something specific to your project, create your own type parser. The rest of this article explains how to create custom type parsers.

To create a custom type parser derive from `ITypeParser` interface. In this example we will create a parser for `System.Uri` class (`System.Uri` is supported by Config.Net, this is just an example):


```csharp
class UriParser : ITypeParser
{
   public IEnumerable<Type> SupportedTypes => new[] { typeof(Uri) };

   public string ToRawString(object value)
   {
      if (value == null) return null;

      return value.ToString();
   }

   public bool TryParse(string value, Type t, out object result)
   {
      if(value == null)
      {
         result = null;
         return false;
      }

      if(t == typeof(Uri))
      {
         Uri uri = new Uri(value);
         result = uri;
         return true;
      }

      result = null;
      return false;
   }
}
```

To use the parser, you need to add it during configuration build stage:


```csharp
public interface ICustomTypes
{
   Uri Address { get; set; }
}

ICustomTypes config = new ConfigurationBuilder<ICustomTypes>()
   .UseTypeParser(new UriParser())
   .Build();
```

Note that custom parsers exist within a context of a configuration interface instance. This allows to use different parsers for the same interface amongst different instances.

When adding a custom parser that is already implemented as built-in parser, the last added parser always wins in terms of priority of use. Therefore using the `UseParser` you can override the standard read/write behavior.