using System;
using System.Reflection;
using System.Collections.Concurrent;
using Config.Net.TypeParsers;

namespace Config.Net
{
   public abstract class SettingsContainer
   {
      private readonly IConfigConfiguration _config = new ContainerConfiguration();

      private readonly ConcurrentDictionary<string, Option> _nameToOption =
         new ConcurrentDictionary<string, Option>();

      private static readonly DefaultParser DefaultParser = new DefaultParser();

      protected SettingsContainer(string namespaceName)
      {
         DiscoverProperties();
      }

      public object Read(Type valueType, string name)
      {
         OnConfigure(_config);

         object result;
         ReadValue(name, valueType, out result);
         return result;
      }

      protected abstract void OnConfigure(IConfigConfiguration configuration);

      private void DiscoverProperties()
      {
         Type t = this.GetType();

         FieldInfo[] properties = t.GetFields();
         foreach(FieldInfo pi in properties)
         {
            if(pi.FieldType.IsSubclassOf(typeof(Option)))
            {
               //hit it
            }
         }
      }

      private bool CanParse(Type t)
      {
         return _config.HasParser(t) || DefaultParser.IsSupported(t);
      }

      private string ReadFirst(string key)
      {
         foreach (IConfigStore store in _config.Stores)
         {
            if (store.CanRead)
            {
               string value = store.Read(key);

               if (value != null) return value;
            }
         }
         return null;
      }

      private bool ReadValue(string keyName, Type valueType, out object result)
      {
         if (!CanParse(valueType))
         {
            throw new ArgumentException("value parser for " + valueType.FullName +
                                        " is not registered and not supported by default parser");
         }

         string value = ReadFirst(keyName);
         if (value == null)
         {
            result = null;
            return false;
         }

         if (DefaultParser.IsSupported(valueType))
         {
            object resultObject;
            if (DefaultParser.TryParse(value, valueType, out resultObject))
            {
               result = resultObject;
               return true;
            }

            result = null;
            return false;
         }

         ITypeParser typeParser = _config.GetParser(valueType);
         return typeParser.TryParse(value, valueType, out result);
      }
   }
}
