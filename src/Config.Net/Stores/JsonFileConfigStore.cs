using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Config.Net.Stores
{
   /// <summary>
   /// Simple JSON storage.
   /// </summary>
   public class JsonFileConfigStore : IConfigStore
   {
      private readonly string _pathName;
      private JObject _jo;

      /// <summary>
      /// Create JSON storage in the file specified in <paramref name="pathName"/>.
      /// </summary>
      /// <param name="pathName">Full or relative path to JSON storage file.</param>
      /// <exception cref="ArgumentNullException"><paramref name="pathName"/> is null.</exception>
      /// <exception cref="IOException">Provided path is not valid.</exception>
      /// <remarks>Storage file does not have to exist, however it will be created as soon as first write performed.</remarks>
      public JsonFileConfigStore(string pathName)
      {
         if (pathName == null) throw new ArgumentNullException(nameof(pathName));

         _pathName = Path.GetFullPath(pathName);   // Allow relative path to JSON file

         ReadJsonFile();
      }

      public void Dispose()
      {
         // nothing to dispose.
      }

      public string Name => $"json:{_pathName}";

      public bool CanRead => true;

      public bool CanWrite => true;

      public string Read(string key)
      {
         if (key == null || _jo == null) return null;

         string path = "$." + key;

         JToken valueToken = _jo.SelectToken(path);

         return GetStringValue(valueToken);
      }

      private string GetStringValue(JToken token)
      {
         if (token == null) return null;

         return token.ToString();
      }

      public void Write(string key, string value)
      {
         if (key == null || _jo == null) return;

         string[] parts = key.Split('.');

         //find the container first
         JToken containerToken = _jo;
         for(int i = 0; i < parts.Length - 1; i++)
         {
            string name = parts[i];
            JToken next = containerToken[name];

            if(next == null)
            {
               next = JObject.Parse("{}");
               containerToken[name] = next;
            }

            containerToken = next;
         }

         //set the value
         string ckey = parts[parts.Length - 1];
         if (value == null)
         {
            //remove the value
            JToken existingValue = containerToken[ckey];

            if (existingValue is JValue jv)
               jv.Parent.Remove();
            else
               existingValue?.Remove();
         }
         else
         {
            containerToken[ckey] = value;
         }

         //rewrite the whole thing
         WriteJsonFile();
      }

      private void ReadJsonFile()
      {
         if(File.Exists(_pathName))
         {
            string json = File.ReadAllText(_pathName);
            _jo = JObject.Parse(json);
         }
         else
         {
            _jo = new JObject();
         }
      }

      private void WriteJsonFile()
      {
         if (_jo == null) return;

         var fi = new FileInfo(_pathName);
         if (!fi.Directory.Exists) fi.Directory.Create();

         string json = _jo.ToString(Formatting.Indented);
         File.WriteAllText(_pathName, json);
      }
   }
}