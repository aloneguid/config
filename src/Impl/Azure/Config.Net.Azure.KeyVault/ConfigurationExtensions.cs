using System;
using Config.Net.Azure.KeyVault;

namespace Config.Net
{
   public static class ConfigurationExtensions
   {
      public static ConfigurationBuilder<TInterface> UseAzureKeyVaultWithManagedIdentity<TInterface>(
         this ConfigurationBuilder<TInterface> builder, Uri vaultUri) where TInterface : class
      {
         builder.UseConfigStore(AzureKeyVaultConfigStore.CreateWithManagedIdentity(vaultUri));

         return builder;
      }
   }
}
