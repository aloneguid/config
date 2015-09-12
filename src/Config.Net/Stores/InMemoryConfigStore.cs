using System.Collections.Generic;

namespace Config.Net.Stores
{
   public class InMemoryConfigStore : IConfigStore
   {
      public string Name { get { return "InMemoryStore"; } }
      public bool CanRead { get { return true; } }
      public bool CanWrite { get { return true; } }

      private readonly Dictionary<string, string> _data = new Dictionary<string, string>(); 

      public string Read(string key)
      {
         if (key == null) return null;

         return _data.ContainsKey(key) ? _data[key] : null;
      }

      public void Write(string key, string value)
      {
         if (key == null) return;

         if (value == null)
         {
            if (_data.ContainsKey(key)) _data.Remove(key);
         }
         else
         {
            _data[key] = value;
         }
      }
   }
}
