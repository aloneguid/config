using System;
using System.Reflection;
using System.Collections.Concurrent;

namespace Config.Net
{
   public abstract class SettingsContainer
   {
      private readonly IConfigConfiguration _config = new ContainerConfiguration();

      private readonly ConcurrentDictionary<string, OptionAttribute> _nameToOption =
         new ConcurrentDictionary<string, OptionAttribute>();

      protected SettingsContainer(string namespaceName)
      {
         OnConfigure(_config);

         DiscoverProperties();
      }

      protected abstract void OnConfigure(IConfigConfiguration configuration);

      private void DiscoverProperties()
      {
         Type t = this.GetType();

         PropertyInfo[] properties = t.GetProperties();
         foreach(PropertyInfo pi in properties)
         {
            bool attributed = false;

            foreach(OptionAttribute attr in pi.GetCustomAttributes<OptionAttribute>())
            {
               attributed = true;

               if (attr.Name == null) attr.Name = pi.Name;
               _nameToOption[attr.Name] = attr;
            }

            if(!attributed)
            {
               _nameToOption[pi.Name] = new OptionAttribute { Name = pi.Name };
            }
         }
      }
   }
}
