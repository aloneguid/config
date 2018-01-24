# AppConfig Store

To configure the store:

```csharp
IMySettings settings = new ConfigurationBuilder<IMySettings>()
   .UseAppConfigSection("myCustomConfigSectionName1")
   .UseAppConfigSection("myCustomConfigSectionName2")
   .Build();
```

Sample usage in App.config (Web.config):

```xml
<configuration>
  <configSections>
    <section name="myCustomConfigSectionName1" type="Config.Net.Stores.Formats.AppConfigSection.AppConfigConfigurationSection, Config.Net" />
    <section name="myCustomConfigSectionName2" type="Config.Net.Stores.Formats.AppConfigSection.AppConfigConfigurationSection, Config.Net" />
  </configSections>
  <myCustomConfigSectionName1>
    <add key="MyConfigKey1" value="MyConfigValue1" />
  </myCustomConfigSectionName1>
  <myCustomConfigSectionName2>
    <add key="MyConfigKey2" value="MyConfigValue2" />
  </myCustomConfigSectionName2>
</configuration>
```

It doesn't have any parameters. It's read-only, and forwards read operation to the standard [ConfigurationManager](https://msdn.microsoft.com/en-us/library/system.configuration.configurationmanager%28v=vs.110%29.aspx?f=255&MSPPError=-2147217396) class.