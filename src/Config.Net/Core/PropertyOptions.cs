using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Config.Net.Core
{
   class PropertyOptions
   {
      public PropertyOptions(string name, object defaultValue)
      {
         Name = name;
         DefaultValue = defaultValue;
      }

      public string Name { get; }

      public object DefaultValue { get; }

      public static Dictionary<string, PropertyOptions> Discover<TInterface>()
      {
         var result = new Dictionary<string, PropertyOptions>();

         PropertyInfo[] properties = typeof(TInterface).GetTypeInfo().GetProperties();

         foreach(PropertyInfo pi in properties)
         {
            OptionAttribute attribute = pi.GetCustomAttribute<OptionAttribute>();

            string name = pi.Name;
            object defaultValue = null;

            if(attribute != null)
            {
               if (attribute.Name != null) name = attribute.Name;
               defaultValue = attribute.DefaultValue;
            }

            result[pi.Name] = new PropertyOptions(name, defaultValue);
         }

         return result;
      }
   }
}
