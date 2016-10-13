using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace Config.Net.Azure
{
   class AzureKeyVaultConfigStore : IConfigStore
   {
      private KeyVaultClient _vaultClient;
      private ClientCredential _credential;
      private readonly string _vaultUri;

      public AzureKeyVaultConfigStore(Uri vaultUri, string azureAadClientId, string azureAadClientSecret)
      {
         _credential = new ClientCredential(azureAadClientId, azureAadClientSecret);
         _vaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(GetAccessToken), GetHttpClient());
         _vaultUri = vaultUri.ToString();
      }

      /// <summary>
      /// Gets the access token
      /// </summary>
      /// <param name="authority"> Authority </param>
      /// <param name="resource"> Resource </param>
      /// <param name="scope"> scope </param>
      /// <returns> token </returns>
      public async Task<string> GetAccessToken(string authority, string resource, string scope)
      {
         var context = new AuthenticationContext(authority, TokenCache.DefaultShared);
         var result = await context.AcquireTokenAsync(resource, _credential);

         return result.AccessToken;
      }

      /// <summary>
      /// Create an HttpClient object that optionally includes logic to override the HOST header
      /// field for advanced testing purposes.
      /// </summary>
      /// <returns>HttpClient instance to use for Key Vault service communication</returns>
      private HttpClient GetHttpClient()
      {
         return new HttpClient();
         //return (HttpClientFactory.Create(new InjectHostHeaderHttpMessageHandler()));
      }

      public bool CanRead
      {
         get
         {
            return true;
         }
      }

      public bool CanWrite
      {
         get
         {
            return true;
         }
      }

      public string Name => "Azure Key Vault";

      public void Dispose()
      {
      }

      public string Read(string key)
      {
         if (key == null) return null;

         Secret secret;

         try
         {
            secret = _vaultClient.GetSecretAsync(_vaultUri, key).Result;
         }
         catch(AggregateException ex)
         {
            KeyVaultClientException ex1 = ex.InnerException as KeyVaultClientException;
            if (ex1 != null && ex1.Status == HttpStatusCode.NotFound)
            {
               secret = null;
            }
            else
            {
               throw;
            }
         }

         return secret == null ? null : secret.Value;
      }

      public void Write(string key, string value)
      {
         if (value == null)
         {
            _vaultClient.DeleteSecretAsync(_vaultUri, key);
         }
         else
         {
            _vaultClient.SetSecretAsync(_vaultUri, key, value).Wait();
         }
      }

   }
}
