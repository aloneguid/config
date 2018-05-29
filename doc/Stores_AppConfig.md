# AppConfig Store

To configure the store:

```csharp
IMySettings settings = new ConfigurationBuilder<IMySettings>()
   .UseAppConfig()
   .Build();
```

It doesn't have any parameters. It's read-only, and forwards read operation to the standard [ConfigurationManager](https://msdn.microsoft.com/en-us/library/system.configuration.configurationmanager%28v=vs.110%29.aspx?f=255&MSPPError=-2147217396) class.

## Collections

Collections are supported by using the [flatline syntax](flatline.md).