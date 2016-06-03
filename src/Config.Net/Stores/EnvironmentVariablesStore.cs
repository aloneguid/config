using System;
using System.Collections.Generic;

namespace Config.Net.Stores
{
   public class EnvironmentVariablesStore : IConfigStore
   {
      public bool CanRead => true;

      public bool CanWrite => true;

      public string Name => "System Environment";


      public string Read(string key)
      {
         foreach(string variant in GetAllKeyVariants(key))
         {
            string value = Environment.GetEnvironmentVariable(variant);
            if (value != null) return value;
         }

         return null;
      }

      public void Write(string key, string value)
      {
         Environment.SetEnvironmentVariable(key, value);
      }

      private IEnumerable<string> GetAllKeyVariants(string key)
      {
         var result = new List<string>();
         result.Add(key);
         result.Add(key.ToUpper().Replace(".", "_"));
         return result;
      }

      public void Dispose()
      {
      }
   }
}