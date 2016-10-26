#if NETFULL
using System;
using Microsoft.Azure;

namespace Config.Net.Azure
{
   class AzureConfigStore : IConfigStore
   {
      public string Name => "Windows Azure Configuration Manager";

      public bool CanRead => true;

      public bool CanWrite => false;

      public string Read(string key)
      {
         if (key == null) return null;
         string value = CloudConfigurationManager.GetSetting(key);
         return value;
      }

      public void Write(string key, string value)
      {
         throw new NotSupportedException();
      }

      public void Dispose()
      {
      }
   }
}
#endif