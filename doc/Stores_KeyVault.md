# Azure Key Vault

[![NuGet](https://img.shields.io/nuget/v/Config.Net.Azure.KeyVault.svg)](https://www.nuget.org/packages/Config.Net.Azure.KeyVault)

## Configuring

You can configure Azure Key Vault store either with [Managed Identity](https://docs.microsoft.com/en-us/azure/active-directory/managed-identities-azure-resources/overview):

```csharp
IMySettings settings = new ConfigurationBuilder<IMySettings>()
   .UseAzureKeyVaultWithManagedIdentity(vaultUri)
   .Build();
```

or in more traditional way with a Service Principal:

```csharp
IMySettings settings = new ConfigurationBuilder<IMySettings>()
   UseAzureKeyVaultWithServicePrincipal(
         this ConfigurationBuilder(vaultUri, clientId, clientSecret)
   .Build();
```