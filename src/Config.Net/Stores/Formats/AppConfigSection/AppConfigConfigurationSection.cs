#if NETFULL
using System.Configuration;

namespace Config.Net.Stores.Formats.AppConfigSection
{
    public class AppConfigConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("", IsRequired = true, IsDefaultCollection = true)]
        public AppConfigSectionElementsCollection Settings
        {
            get => (AppConfigSectionElementsCollection)this[""];
            set => this[""] = value;
        }
    }
}
#endif