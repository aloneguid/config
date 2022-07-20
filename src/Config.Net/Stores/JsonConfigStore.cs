#if NET5_0_OR_GREATER || NETCOREAPP3_1
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using Config.Net.Core;

namespace Config.Net.Stores
{
   /// <summary>
   /// Simple JSON storage using System.Text.Json
   /// </summary>
   public class JsonConfigStore : IConfigStore
   {
      private readonly string? _pathName;
      private readonly Dictionary<string, JsonElement>? _jsonMap;

      /// <summary>
      /// Create JSON storage in the file specified in <paramref name="name"/>.
      /// </summary>
      /// <param name="name">Name of the file, either path to JSON storage file, or json file content.</param>
      /// <param name="isFilePath">Set to true if <paramref name="name"/> specifies file name, otherwise false. </param>
      /// <exception cref="ArgumentNullException"><paramref name="name"/> is null.</exception>
      /// <exception cref="IOException">Provided path is not valid.</exception>
      /// <remarks>Storage file does not have to exist, however it will be created as soon as first write performed.</remarks>
      public JsonConfigStore(string name, bool isFilePath)
      {
         if (name == null) throw new ArgumentNullException(nameof(name));

         if(isFilePath)
         {
            _pathName = Path.GetFullPath(name);   // Allow relative path to JSON file
            _jsonMap = ReadJsonFile(_pathName);
         }
         else
         {
            _jsonMap = ReadJsonString(name);
         }
      }

      public void Dispose()
      {
         // nothing to dispose.
      }

      public string Name => "json";

      public bool CanRead => true;

      public bool CanWrite => false;

      private static bool TryGetElementAt(JsonElement mother, int index, out JsonElement result)
      {
         result = mother;

         if (index >= mother.GetArrayLength()) return false;

         int idx = 0;
         foreach(JsonElement e in mother.EnumerateArray())
         {
            if(idx++ == index)
            {
               result = e;
               return true;
            }
         }

         return false;
      }

      public string? Read(string key)
      {
         if (string.IsNullOrEmpty(key) || _jsonMap == null) return null;

         bool isLength = OptionPath.TryStripLength(key, out string? strippedKey);
         if (strippedKey == null) return null;

         string[] parts = strippedKey.Split('.');
         if (parts.Length == 0) return null;

         // navigate JSON manually (there's no jsonpath support)

         string? safePart;
         bool isIndex = OptionPath.TryStripIndex(parts[0], out safePart, out int partIndex);
         if (safePart == null) return null;

         if (!_jsonMap.TryGetValue(safePart, out JsonElement current)) return null;
         if(isIndex)
         {
            if (!TryGetElementAt(current, partIndex, out current)) return null;
         }

         bool found = true;   // because previous lookup was successful (prev line)
         for (int i = 1; i < parts.Length; i++)
         {
            if(current.ValueKind == JsonValueKind.Object)
            {
               found = current.TryGetProperty(parts[i], out JsonElement next);
               if(found)
                  current = next;
               else
                  break;
            }
            else if(current.ValueKind == JsonValueKind.Array)
            {
               throw new NotSupportedException();
            }
            else
            {
               found = false;
               break;
            }
         }

         if (!found) return null;

         if(current.ValueKind == JsonValueKind.Array)
         {
            if (isLength)
               return current.GetArrayLength().ToString();
            else return null;
         }
         else
         {
            return current.ToString();
         }
      }

      public void Write(string key, string? value)
      {
         throw new NotSupportedException();
      }

      private static Dictionary<string, JsonElement>? ReadJsonFile(string fileName)
      {
         if(File.Exists(fileName))
         {
            string json = File.ReadAllText(fileName);
            return ReadJsonString(json);
         }

         return null;
      }

      private static Dictionary<string, JsonElement>? ReadJsonString(string jsonString)
      {
         return JsonSerializer.Deserialize<Dictionary<string, JsonElement>?>(jsonString);
      }
   }
}
#endif