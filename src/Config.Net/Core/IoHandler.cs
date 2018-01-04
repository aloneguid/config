using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Config.Net.Core
{
   class IoHandler
   {
      private readonly IEnumerable<IConfigStore> _stores;
      private readonly ValueHandler _valueHandler;
      private readonly TimeSpan _cacheInterval;
      private readonly ConcurrentDictionary<string, LazyVar<object>> _keyToValue = new ConcurrentDictionary<string, LazyVar<object>>();

      public IoHandler(IEnumerable<IConfigStore> stores, ValueHandler valueHandler, TimeSpan cacheInterval)
      {
         _stores = stores ?? throw new ArgumentNullException(nameof(stores));
         _valueHandler = valueHandler ?? throw new ArgumentNullException(nameof(valueHandler));
         _cacheInterval = cacheInterval;
      }

      public ValueHandler ValueHandler => _valueHandler;

      public object Read(Type baseType, string path, object defaultValue)
      {
         if(!_keyToValue.TryGetValue(path, out LazyVar<object> value))
         {
            _keyToValue[path] = new LazyVar<object>(_cacheInterval, () => ReadNonCached(baseType, path, defaultValue));
         }

         return _keyToValue[path].GetValue();
      }

      public void Write(Type baseType, string path, object value)
      {
         string valueToWrite = _valueHandler.ConvertValue(baseType, value);

         foreach (IConfigStore store in _stores.Where(s => s.CanWrite))
         {
            store.Write(path, valueToWrite);
         }
      }

      private object ReadNonCached(Type baseType, string path, object defaultValue)
      {
         string rawValue = ReadFirstValue(path);

         return _valueHandler.ParseValue(baseType, rawValue, defaultValue);
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
