using System;
using System.Collections.Generic;
using Config.Net.Core;

namespace Config.Net.Stores
{
   /// <summary>
   /// Uses system environment variables
   /// </summary>
   class EnvironmentVariablesStore : IConfigStore
   {
      /// <summary>
      /// Readable
      /// </summary>
      public bool CanRead => true;

      /// <summary>
      /// Writeable
      /// </summary>
      public bool CanWrite => true;

      /// <summary>
      /// Store name
      /// </summary>
      public string Name => "System Environment";

      /// <summary>
      /// Reads value by key
      /// </summary>
      public string? Read(string key)
      {
         if (key == null) return null;

         foreach(string variant in GetAllKeyVariants(key))
         {
            if (FlatArrays.IsArrayLength(variant, k => Environment.GetEnvironmentVariable(k), out int length))
            {
               return length.ToString();
            }

            if (FlatArrays.IsArrayElement(variant, k => Environment.GetEnvironmentVariable(k), out string? element))
            {
               return element;
            }

            string? value = Environment.GetEnvironmentVariable(variant);
            if (value != null) return value;
         }

         return null;
      }

      /// <summary>
      /// Writes value by key
      /// </summary>
      public void Write(string key, string? value)
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

      /// <summary>
      /// Nothing to dispose
      /// </summary>
      public void Dispose()
      {
      }
   }
}