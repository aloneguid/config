# Azure Table Store

This store come from a separate NuGet package [Config.Net.Azure](https://www.nuget.org/packages/Config.Net.Azure)

To configure the store:

```csharp
protected override void OnConfigure(IConfigConfiguration configuration)
{
	configuration.UseAzureKeyVault(new Uri("vault_uri"), "Azure AD App Client ID", "Azure AD App Client Secret");
}
```

This store uses Azure Key Vault to read and store secrets as configuration values. The store is fully functional, however it's still experimental as we're trying to figure out the best way to work with permissions and give the best error handling.

The documentation is in progress...