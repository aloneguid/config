using System;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Azure.Services.AppAuthentication;

namespace Config.Net.Azure.KeyVault
{
   public class AzureKeyVaultConfigStore : IConfigStore
   {
      private readonly string _vaultUri;
      private readonly KeyVaultClient _client;

      private AzureKeyVaultConfigStore(Uri vaultUri, KeyVaultClient client)
      {
         _vaultUri = vaultUri.ToString().Trim('/');
         _client = client;
      }

      public static AzureKeyVaultConfigStore CreateWithManagedIdentity(Uri vaultUri)
      {
         var azureServiceTokenProvider = new AzureServiceTokenProvider();

         var client = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));

         return new AzureKeyVaultConfigStore(vaultUri, client);
      }

      public bool CanRead => true;

      public bool CanWrite => false;

      public void Dispose()
      {

      }

      public string Read(string key)
      {
         if (key == null) return null;

         SecretBundle secret = _client.GetSecretAsync(_vaultUri, key).Result;

         return secret.Value;
      }

      public void Write(string key, string value)
      {
         _client.SetSecretAsync(_vaultUri, key, value).Wait();
      }
   }
}
