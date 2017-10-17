using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NetBox.Caching;

namespace Config.Net.Core
{
   class IoHandler
   {
      private readonly IEnumerable<IConfigStore> _stores;
      private readonly TimeSpan _cacheInterval;
      private readonly ConcurrentDictionary<string, LazyVar<object>> _keyToValue = new ConcurrentDictionary<string, LazyVar<object>>();

      public IoHandler(IEnumerable<IConfigStore> stores, TimeSpan cacheInterval)
      {
         _stores = stores ?? throw new ArgumentNullException(nameof(stores));
         _cacheInterval = cacheInterval;
      }

      public object Read(PropertyOptions property)
      {
         if(!_keyToValue.TryGetValue(property.Name, out LazyVar<object> value))
         {
            _keyToValue[property.Name] = new LazyVar<object>(_cacheInterval, () => ReadNonCached(property));
         }

         return _keyToValue[property.Name].GetValue();
      }

      public void Write(PropertyOptions property, object value)
      {
         string valueToWrite = ValueHandler.Default.ConvertValue(property, value);

         foreach (IConfigStore store in _stores.Where(s => s.CanWrite))
         {
            store.Write(property.Name, valueToWrite);
         }
      }

      private object ReadNonCached(PropertyOptions property)
      {
         string rawValue = ReadFirstValue(property.Name);

         return ValueHandler.Default.ParseValue(property, rawValue);
      }

      private string ReadFirstValue(string key)
      {
         foreach (IConfigStore store in _stores)
         {
            if (store.CanRead)
            {
               string value = store.Read(key);

               if (value != null) return value;
            }
         }
         return null;
      }



   }
}
