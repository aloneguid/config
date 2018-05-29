using System;
using System.Collections.Generic;
using System.Text;

namespace Config.Net.Core
{
   static class Extensions
   {
      public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
      {
         if (!dictionary.TryGetValue(key, out TValue value)) return default(TValue);

         return value;
      }
   }
}
