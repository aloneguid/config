using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

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

      public object Read(PropertyOptions property, string name = null)
      {
         if (name == null) name = property.StoreName;

         if(!_keyToValue.TryGetValue(name, out LazyVar<object> value))
         {
            _keyToValue[name] = new LazyVar<object>(_cacheInterval, () => ReadNonCached(property, name));
         }

         return _keyToValue[name].GetValue();
      }

      public void Write(PropertyOptions property, object value, string name = null)
      {
         if (name == null) name = property.StoreName;

         string valueToWrite = ValueHandler.Default.ConvertValue(property, value);

         foreach (IConfigStore store in _stores.Where(s => s.CanWrite))
         {
            store.Write(name, valueToWrite);
         }
      }

      private object ReadNonCached(PropertyOptions property, string name)
      {
         string rawValue = ReadFirstValue(name);

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
