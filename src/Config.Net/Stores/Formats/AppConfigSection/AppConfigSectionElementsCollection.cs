#if NETFULL
using System.Configuration;

namespace Config.Net.Stores.Formats.AppConfigSection
{
   class AppConfigSectionElementsCollection : ConfigurationElementCollection
   {
      protected override ConfigurationElement CreateNewElement()
      {
         return new AppConfigSectionElement();
      }

      protected override object GetElementKey(ConfigurationElement element)
      {
         return ((AppConfigSectionElement)element).Key;
      }
   }
}
#endif