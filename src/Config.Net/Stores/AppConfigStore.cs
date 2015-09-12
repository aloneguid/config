using System;
using System.Configuration;

namespace Config.Net.Stores
{
   public class AppConfigStore : IConfigStore
   {
      public string Name { get { return "App.config"; } }

      public bool CanRead { get { return true; } }

      public bool CanWrite { get { return false; } }

      public string Read(string key)
      {
         if (key == null) return null;
         string value = ConfigurationManager.AppSettings[key];
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
