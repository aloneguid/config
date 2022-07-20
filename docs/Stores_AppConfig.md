# AppConfig Store

To configure the store:

```csharp
IMySettings settings = new ConfigurationBuilder<IMySettings>()
   .UseAppConfig()
   .Build();
```

It doesn't have any parameters. It's read-only, and forwards read operation to the standard [ConfigurationManager](https://msdn.microsoft.com/en-us/library/system.configuration.configurationmanager%28v=vs.110%29.aspx?f=255&MSPPError=-2147217396) class.

- Keys are mapped straight to `<appSettings>` elements.
- If a key is not found in _appSettings_, an attempt will be made to find it in `<connectionStrings>`
- If it's still not found, and attempt will be made to find a section with a name before the first dot separator, and read the key from there.

To demonstrate this, consider the following example *app.config*:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
   <configSections>
      <section name="MySection" type="System.Configuration.NameValueSectionHandler"/>
   </configSections>
   <appSettings>
      <add key="AppKey" value="TestValue"/>
   </appSettings>
   <connectionStrings>
      <add name="MyConnection" connectionString="testconn"/>
   </connectionStrings>
   <MySection>
      <add key="MyKey" value="MyCustomValue"/>
   </MySection>
</configuration>
```

It can be mapped to configuration interface as follows:

```csharp
   public interface IConfig
   {
      string AppKey { get; }

      string MyConnection { get; }

      [Option(Alias = "MySection.MyKey")]
      string MySectionKey { get; }
   }
```

## Collections

Collections are supported by using the [flatline syntax](flatline.md).