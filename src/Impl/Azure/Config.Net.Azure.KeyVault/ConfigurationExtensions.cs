using System;
using Config.Net.Azure.KeyVault;

namespace Config.Net
{
   public static class ConfigurationExtensions
   {
      /// <summary>
      /// Use Azure Key Vault by authenticating with Managed Identity
      /// </summary>
      /// <typeparam name="TInterface"></typeparam>
      /// <param name="builder"></param>
      /// <param name="vaultUri">Key Vault URI</param>
      /// <returns></returns>
      public static ConfigurationBuilder<TInterface> UseAzureKeyVaultWithManagedIdentity<TInterface>(
         this ConfigurationBuilder<TInterface> builder, Uri vaultUri) where TInterface : class
      {
         builder.UseConfigStore(AzureKeyVaultConfigStore.CreateWithManagedIdentity(vaultUri));

         return builder;
      }
      /// <summary>
      /// Use Azure Key Vault by authenticating with service principal
      /// </summary>
      /// <typeparam name="TInterface"></typeparam>
      /// <param name="builder"></param>
      /// <param name="vaultUri">Key Vault URI</param>
      /// <param name="clientId">Service Principal Client ID</param>
      /// <param name="clientSecret">Service Principal Secret</param>
      /// <returns></returns>
      public static ConfigurationBuilder<TInterface> UseAzureKeyVaultWithServicePrincipal<TInterface>(
         this ConfigurationBuilder<TInterface> builder, Uri vaultUri, string clientId, string clientSecret) where TInterface : class
      {
         builder.UseConfigStore(AzureKeyVaultConfigStore.CreateWithPrincipal(vaultUri, clientId, clientSecret));

         return builder;
      }

   }
}
