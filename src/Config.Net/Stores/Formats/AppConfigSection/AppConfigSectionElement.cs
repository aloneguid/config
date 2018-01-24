#if NETFULL
using System.Configuration;

namespace Config.Net.Stores.Formats.AppConfigSection
{
   public class AppConfigSectionElement : ConfigurationElement
   {
      private const string KeyAttributeName = "key";
      private const string ValueAttributeName = "value";

      [ConfigurationProperty(KeyAttributeName, IsKey = true, IsRequired = true)]
      public string Key
      {
         get => (string)base[KeyAttributeName];
         set => base[KeyAttributeName] = value;
      }

      [ConfigurationProperty(ValueAttributeName, IsRequired = true)]
      public string Value
      {
         get => (string)base[ValueAttributeName];
         set => base[ValueAttributeName] = value;
      }
   }
}
#endif