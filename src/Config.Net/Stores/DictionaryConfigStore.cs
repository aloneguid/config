using System;
using System.Collections.Generic;
using System.Text;
using Config.Net.Core;

namespace Config.Net.Stores
{
   class DictionaryConfigStore : IConfigStore
   {
      private readonly IDictionary<string, string> _container;

      public DictionaryConfigStore(IDictionary<string, string> container = null)
      {
         _container = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

         if(container != null)
         {
            foreach(KeyValuePair<string, string> item in container)
            {
               _container[item.Key] = item.Value;
            }
         }
      }

      public bool CanRead => true;

      public bool CanWrite => true;

      public void Dispose()
      {

      }

      public string Read(string key)
      {
         if (key == null) return null;

         if (FlatArrays.IsArrayLength(key, k => _container.GetValueOrDefaultInternal(k), out int length))
         {
            return length.ToString();
         }

         if (FlatArrays.IsArrayElement(key, k => _container.GetValueOrDefaultInternal(k), out string element))
         {
            return element;
         }

         return _container.GetValueOrDefaultInternal(key);
      }

      public void Write(string key, string value)
      {
         if (key == null) return;

         if(value == null)
         {
            _container.Remove(key);
         }

         _container[key] = value;
      }
   }
}
