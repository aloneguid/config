using System.Collections.Generic;

namespace Config.Net.Tests
{
   class TestStore : IConfigStore
   {
      public Dictionary<string, string> Map { get; private set; }
 
      public TestStore(bool canWrite = true)
      {
         CanWrite = canWrite;
         Map = new Dictionary<string, string>();
      }

      public string Name { get { return "test";  } }
      public bool CanRead { get { return true; } }
      public bool CanWrite { get; set; }

      public string Read(string key)
      {
         if (Map.ContainsKey(key)) return Map[key];
         return null;
      }

      public void Write(string key, string value)
      {
         Map[key] = value;
      }

      public void Dispose()
      {
      }
   }
}
