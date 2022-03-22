using System;
using System.Collections.Specialized;
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

         //first, look at appsettings and connection strings
         string value = ConfigurationManager.AppSettings[key] ?? ConfigurationManager.ConnectionStrings[key]?.ConnectionString;

         if(value == null)
         {
            int idx = key.IndexOf('.');
            if(idx != -1)
            {
               string sectionName = key.Substring(0, idx);
               if(ConfigurationManager.GetSection(sectionName) is NameValueCollection nvSc)
               {
                  string keyName = key.Substring(idx + 1);
                  value = nvSc[keyName];
               }
            }
         }

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