using System.Collections.Generic;

namespace Config.Net.Core
{
   static class Extensions
   {
      public static TValue GetValueOrDefaultInternal<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
      {
         if (!dictionary.TryGetValue(key, out TValue value)) return default(TValue);

         return value;
      }
   }
}
