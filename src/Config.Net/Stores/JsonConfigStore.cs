using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using Config.Net.Core;

namespace Config.Net.Stores
{
   /// <summary>
   /// Simple JSON storage using System.Text.Json
   /// </summary>
   public class JsonConfigStore : IConfigStore
   {
      private readonly string? _pathName;
      private JsonNode? _j;

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
            _j = ReadJsonFile(_pathName);
         }
         else
         {
            _j = ReadJsonString(name);
         }
      }

      public void Dispose()
      {
         // nothing to dispose.
      }

      public string Name => "json";

      public bool CanRead => true;

      public bool CanWrite => _pathName != null;

      public string? Read(string rawKey)
      {
         if (string.IsNullOrEmpty(rawKey) || _j == null) return null;

         bool isLength = OptionPath.TryStripLength(rawKey, out string? key);
         if (key == null) return null;

         string[] parts = key.Split('.');
         if (parts.Length == 0) return null;

         JsonNode? node = _j;
         foreach(string rawPart in parts)
         {
            bool isIndex = OptionPath.TryStripIndex(rawPart, out string? part, out int partIndex);
            if (part == null) return null;

            node = node![part];
            if (node == null) return null;

            if(isIndex)
            {
               if (!(node is JsonArray ja)) return null;

               if (partIndex < ja.Count)
               {
                  node = ja[partIndex];
               }
               else
                  return null;
            }
         }

         if (isLength)
            return node is JsonArray ja ? ja.Count.ToString() : null;

         return node!.ToString();
      }

      public void Write(string key, string? value)
      {
         if (string.IsNullOrEmpty(_pathName))
            throw new InvalidOperationException("please specify file name for writeable config");

         if (_j == null) _j = new JsonObject();

         // navigate to target element, create if needed
         string[] parts = key.Split('.');
         if (parts.Length == 0) return;

         JsonNode? node = _j;
         string? lastPart = null;
         foreach (string rawPart in parts)
         {
            bool isIndex = OptionPath.TryStripIndex(rawPart, out string? part, out int partIndex);
            if (part == null) return;
            lastPart = part;

            JsonNode? nextNode = node[part];

            if (isIndex)
            {
               throw new NotImplementedException();
            }
            else
            {
               if(nextNode == null)
               {
                  //create missing node
                  nextNode = new JsonObject();
                  node[part] = nextNode;
               }
            }

            node = nextNode;

         }

         JsonObject? parent = node.Parent as JsonObject;
         parent!.Remove(lastPart!);
         parent![lastPart!] = JsonValue.Create(value);

         string js = _j.ToJsonString(new JsonSerializerOptions { WriteIndented = true });
         File.WriteAllText(_pathName, js);
      }

      private static JsonNode? ReadJsonFile(string fileName)
      {
         if(File.Exists(fileName))
         {
            string json = File.ReadAllText(fileName);
            return ReadJsonString(json);
         }

         return null;
      }

      private static JsonNode? ReadJsonString(string jsonString)
      {
         return JsonNode.Parse(jsonString);
      }
   }
}