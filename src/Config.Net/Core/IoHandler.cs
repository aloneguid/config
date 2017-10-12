using System;
using System.Collections.Generic;
using System.Linq;

namespace Config.Net.Core
{
   class IoHandler
   {
      private readonly IEnumerable<IConfigStore> _stores;

      public IoHandler(IEnumerable<IConfigStore> stores)
      {
         _stores = stores ?? throw new ArgumentNullException(nameof(stores));
      }

      public object Read(PropertyOptions property)
      {
         return ReadNonCached(property);
      }

      public void Write(PropertyOptions property, object value)
      {
         string valueToWrite = ValueHandler.Default.ConvertValue(property, value);

         foreach (IConfigStore store in _stores.Where(s => s.CanWrite))
         {
            store.Write(property.Name, valueToWrite);
         }
      }

      public object ReadNonCached(PropertyOptions property)
      {
         //assume configuration is valid

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
