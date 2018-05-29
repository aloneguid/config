using System.Collections.Generic;
using Config.Net.Core;

namespace Config.Net.Stores
{
   class InMemoryConfigStore : IConfigStore
   {
      public string Name => "InMemoryStore";
      public bool CanRead => true;
      public bool CanWrite => true;

      private readonly Dictionary<string, string> _data = new Dictionary<string, string>();

      public string Read(string key)
      {
         if(key == null) return null;

         if (FlatArrays.IsArrayLength(key, k => _data.GetValueOrDefault(k), out int length))
         {
            return length.ToString();
         }

         if (FlatArrays.IsArrayElement(key, k => _data.GetValueOrDefault(k), out string element))
         {
            return element;
         }


         return _data.GetValueOrDefault(key);
      }

      public void Write(string key, string value)
      {
         if(key == null) return;

         if(value == null)
         {
            if(_data.ContainsKey(key)) _data.Remove(key);
         }
         else
         {
            _data[key] = value;
         }
      }

      public void Dispose()
      {
      }
   }
}
