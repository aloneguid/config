using System.Collections.Generic;

namespace Config.Net.Stores
{
   class InMemoryConfigStore : IConfigStore
   {
      public string Name => "InMemoryStore";
      public bool CanRead => true;
      public bool CanWrite { get; set; }

      private readonly Dictionary<string, string> _data = new Dictionary<string, string>();

      public InMemoryConfigStore(bool canWrite = true)
      {
         CanWrite = canWrite;
      }

      public string Read(string key)
      {
         if(key == null) return null;

         return _data.ContainsKey(key) ? _data[key] : null;
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
