# Environment Variables Store

To configure the store:

```csharp
IMySettings settings = new ConfigurationBuilder<IMySettings>()
   .UseEnvironmentVariables()
   .Build();
```

 This store works with system environment variables, the ones you get on Windows **cmd.exe** by typing `set` or in PowerShell by typing `Get-ChildItem Env:` or on Unix base systems `env`.

The store supports reading and writing environment variables.

> Note: Some systems like Visual Studio Team System Build replace dots (.) with underscores (_) when defining a variable. To overcome this the store will try to read the variable in both variants.

## Collections

Collections are supported by using the [flatline syntax](flatline.md).