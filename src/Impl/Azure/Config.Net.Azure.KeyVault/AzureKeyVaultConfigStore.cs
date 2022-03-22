using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using NetBox.Extensions;

namespace Config.Net.Azure.KeyVault
{
   public class AzureKeyVaultConfigStore : IConfigStore
   {
      private readonly string _vaultUri;
      private readonly KeyVaultClient _client;

      private AzureKeyVaultConfigStore(Uri vaultUri, KeyVaultClient client)
      {
         if (vaultUri == null)
         {
            throw new ArgumentNullException(nameof(vaultUri));
         }

         _vaultUri = vaultUri.ToString().Trim('/');
         _client = client;
      }

      public static AzureKeyVaultConfigStore CreateWithManagedIdentity(Uri vaultUri)
      {
         var azureServiceTokenProvider = new AzureServiceTokenProvider();

         var client = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));

         return new AzureKeyVaultConfigStore(vaultUri, client);
      }

      public static AzureKeyVaultConfigStore CreateWithPrincipal(Uri vaultUri, string azureAadClientId, string azureAadClientSecret)
      {
         if (string.IsNullOrEmpty(azureAadClientId))
         {
            throw new ArgumentException("message", nameof(azureAadClientId));
         }

         if (string.IsNullOrEmpty(azureAadClientSecret))
         {
            throw new ArgumentException("message", nameof(azureAadClientSecret));
         }

         var credential = new ClientCredential(azureAadClientId, azureAadClientSecret);

         var client = new KeyVaultClient(
            new KeyVaultClient.AuthenticationCallback( (authority, resource, scope) => GetAccessToken(authority, resource, scope, credential) ),
            GetHttpClient());

         return new AzureKeyVaultConfigStore(vaultUri, client);
      }

      private static async Task<string> GetAccessToken(string authority, string resource, string scope, ClientCredential credential)
      {
         var context = new AuthenticationContext(authority, TokenCache.DefaultShared);

         AuthenticationResult result = await context.AcquireTokenAsync(resource, credential);

         return result.AccessToken;
      }

      private static HttpClient GetHttpClient()
      {
         return new HttpClient();
      }

      public bool CanRead => true;

      public bool CanWrite => false;

      public void Dispose()
      {

      }

      public string Read(string key)
      {
         if (key == null) return null;
         key = key.UrlEncode();

         try
         {
            SecretBundle secret = _client.GetSecretAsync(_vaultUri, key).Result;

            return secret.Value;
         }
         catch (AggregateException agEx) when(agEx.InnerException is KeyVaultErrorException kvEx && kvEx.Response.StatusCode == HttpStatusCode.NotFound)
         {
            //ignore, the secret is simply not found
            return null;
         }
      }

      public void Write(string key, string value)
      {
         if (key == null)
         {
            throw new ArgumentNullException(nameof(key));
         }

         key = key.UrlEncode();

         _client.SetSecretAsync(_vaultUri, key, value).Wait();
      }
   }
}
