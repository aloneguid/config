using System;
using Microsoft.Azure;

namespace Config.Net.Azure
{
   public class AzureConfigStore : IConfigStore
   {
      public string Name
      {
         get { return "Windows Azure"; }
      }

      public bool CanRead
      {
         get { return true; }
      }

      public bool CanWrite
      {
         get { return false; }
      }

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
   }
}
