# Creating Custom Type Parsers

In cases `Config.Net` does not support data type you work with there are two recommended ways to fix it:

- If this type is widely used i.e. part of .NET, contribute to `Config.Net` source code.
- If this is something specific to your project, create your own type parser. The rest of this article explains how to create custom type parsers.

To create a custom type parser derive from `ITypeParser` interface. In this example we will create a parser for `System.Uri` class:


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

In order to use the parser add it to the settings container configuration:


```csharp
protected override void OnConfigure(IConfigConfiguration configuration)
{
	configuration.AddParser(new UriParser());
}
```

Custom parsers exist within a context of a settings container. If you have multiple containers and want to have it it all of them you need to add the parser separately.