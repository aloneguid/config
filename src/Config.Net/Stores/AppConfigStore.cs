using System;
using System.Configuration;

namespace Config.Net.Stores
{
   /// <summary>
   /// Standard app.config (web.config) configuration store. Read-only.
   /// </summary>
   class AppConfigStore : IConfigStore
   {
      public string Name => "App.config";

      public bool CanRead => true;

      public bool CanWrite => false;

      public string Read(string key)
      {
         if(key == null) return null;
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
