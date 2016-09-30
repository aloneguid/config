using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;

namespace Config.Net.Azure
{
   class AzureKeyVaultConfigStore : IConfigStore
   {
      private KeyVaultClient _vaultClient;

      public AzureKeyVaultConfigStore()
      {

      }

      public bool CanRead
      {
         get
         {
            throw new NotImplementedException();
         }
      }

      public bool CanWrite
      {
         get
         {
            throw new NotImplementedException();
         }
      }

      public string Name
      {
         get
         {
            throw new NotImplementedException();
         }
      }

      public void Dispose()
      {
         throw new NotImplementedException();
      }

      public string Read(string key)
      {
         throw new NotImplementedException();
      }

      public void Write(string key, string value)
      {
         throw new NotImplementedException();
      }
   }
}
