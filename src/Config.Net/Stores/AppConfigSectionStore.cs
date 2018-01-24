#if NETFULL
using System;
using System.Linq;
using System.Configuration;
using Config.Net.Stores.Formats.AppConfigSection;

namespace Config.Net.Stores
{
   /// <summary>
   /// Standard app.config (web.config) custom configuration section store. Read-only.
   /// </summary>
   class AppConfigSectionStore : IConfigStore
   {
      private readonly string _sectionName;

      public UseAppConfigSectionStore(string sectionName)
      {
         _sectionName = sectionName;
      }

      public string Name => "App.config";

      public bool CanRead => true;

      public bool CanWrite => false;

      public string Read(string key)
      {
         if (key == null)
         {
            return null;
         }

         if (!(ConfigurationManager.GetSection(_sectionName) is AppConfigConfigurationSection configSection))
         {
            return null;
         }

         var element = configSection.Settings.Cast<AppConfigSectionElement>().SingleOrDefault(x => x.Key == key);
         return element?.Value;
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