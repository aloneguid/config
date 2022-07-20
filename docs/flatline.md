# Flatline Syntax

## Complex Structures

Many providers do not support nested structures. Suppose you have the following configuration declaration:

```csharp
//collection element
public interface IArrayElement
{
   string Username { get; }

   string Password { get; }
}

//top level configuration
public interface IConfig
{
   IEnumerable<IArrayElement> Creds { get; }
}
```

and you would like to to pass two elements which in json represent as follows:

```json
"Creds": [
   {
      "Username": "user1",
      "Password": "pass1"
   },
   {
      "Username": "user2",
      "Password":  "pass2"
   }
]
```

however you are using comman-line configuration provider which apparently has no nested structures. Config.Net sugggest something called a **flatline syntax** to be able to still use flat like providers and pass nested structures. an example above will translate to:

```bash
myapp.exe Creds.$l=2 Creds[0].Username=user1 Creds[0].Password=pass1 Creds[1].Username=user2 Creds[1].Password=pass2
```

which looks really expressive, however it still allows you to utilise nested structures. 

In practice you probably wouldn't use command like to pass large nested structures, but rather override some of the default parameters.

## Simple Structures

Simple structures can be represented by combining all the values on one single line. For instance the following configuration:

```csharp
public interface ISimpleArrays
{
   IEnumerable<int> Numbers { get; }
}
```

can be mapped to the following command line:

```bash
myapp.exe Numbers="1 2 3"
```

The syntax for providing multiple values in one parameter is identical to the one described in [command-line storage](Stores_CommandLine.md).