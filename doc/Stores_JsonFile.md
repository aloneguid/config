# JSON File Store

To configure the store:

```csharp
protected override void OnConfigure(IConfigConfiguration configuration)
{
    configuration.UseJsonFile("full path to file.json");
}
```

The store supports reading and writing.

In the simplest form every key in the JSON file corresponds to the name of an option. For instance a definition

```csharp

public class AllSettings : SettingsContainer
    {
        public readonly Option<string> AuthClientId = new Option<string>("Id");

        public readonly Option<string> AuthClientSecret = new Option<string>("Secret");

        protected override void OnConfigure(IConfigConfiguration configuration)
        {
            configuration.UseJsonConfig(@"your_path_to_config.json");
        }
    }
```

will correspond to the following JSON file:

``` json
{
   "AuthClientId":"Id",
   "AuthClientSecret":"Secret"
}
```