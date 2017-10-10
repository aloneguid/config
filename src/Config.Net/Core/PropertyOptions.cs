using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Config.Net.Core
{
   class PropertyOptions
   {
      public PropertyOptions(string name, Type type, object defaultValue)
      {
         Name = name;

         Type = type;

         TypeInfo ti = type.GetTypeInfo();
         if(ti.IsClass)
         {
            BaseType = type;
         }
         else
         {
            if(ti.IsGenericType && ti.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
               BaseType = ti.GenericTypeArguments[0];
            }
            else
            {
               BaseType = type;
            }
         }

         DefaultValue = defaultValue;
      }

      public string Name { get; }

      public Type Type { get; }

      public Type BaseType { get; }

      public object DefaultValue { get; }

      public static Dictionary<string, PropertyOptions> Discover<TInterface>()
      {
         var result = new Dictionary<string, PropertyOptions>();

         IEnumerable<PropertyInfo> properties = typeof(TInterface).GetTypeInfo().DeclaredProperties;
         //PropertyInfo[] properties = typeof(TInterface).GetTypeInfo().GetProperties();

         foreach(PropertyInfo pi in properties)
         {
            OptionAttribute attribute = pi.GetCustomAttribute<OptionAttribute>();

            string name = pi.Name;
            object defaultValue = null;

            if(attribute != null)
            {
               if (attribute.Alias != null) name = attribute.Alias;
               defaultValue = attribute.DefaultValue;
            }

            if (defaultValue == null) defaultValue = GetDefaultValue(pi.PropertyType);

            result[pi.Name] = new PropertyOptions(name, pi.PropertyType, defaultValue);
         }

         return result;
      }

      private static object GetDefaultValue(Type t)
      {
         if (t.GetTypeInfo().IsValueType) return Activator.CreateInstance(t);

         return null;
      }
   }
}
