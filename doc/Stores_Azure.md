# Azure Store

This store come from a separate NuGet package [Config.Net.Azure](https://www.nuget.org/packages/Config.Net.Azure)

To configure the store:

```csharp
protected override void OnConfigure(IConfigConfiguration configuration)
{
	configuration.UseAzureConfigStore();
}
```

The store is read-only and forwards the read action to Azure SDK's built-in [CloudConfigurationManager](https://msdn.microsoft.com/en-us/library/azure/mt634650.aspx?f=255&MSPPError=-2147217396).