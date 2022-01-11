using System;
using System.IO;
using System.Text;
using Config.Net.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Config.Net.Json.Stores
{
   /// <summary>
   /// Simple JSON storage.
   /// </summary>
   public class JsonFileConfigStore : IConfigStore
   {
      private readonly string _pathName;
      private readonly JObject _jo;

      /// <summary>
      /// Create JSON storage in the file specified in <paramref name="name"/>.
      /// </summary>
      /// <param name="name">Name of the file, either path to JSON storage file, or json file content.</param>
      /// <param name="isFilePath">Set to true if <paramref name="name"/> specifies file name, otherwise false. </param>
      /// <exception cref="ArgumentNullException"><paramref name="name"/> is null.</exception>
      /// <exception cref="IOException">Provided path is not valid.</exception>
      /// <remarks>Storage file does not have to exist, however it will be created as soon as first write performed.</remarks>
      public JsonFileConfigStore(string name, bool isFilePath)
      {
         if (name == null) throw new ArgumentNullException(nameof(name));

         if(isFilePath)
         {
            _pathName = Path.GetFullPath(name);   // Allow relative path to JSON file

            if (!File.Exists(_pathName))
            {
               throw new FileNotFoundException("File does not exist", _pathName);
            }

            _jo = ReadJsonFile(_pathName);
            CanWrite = true;
         }
         else
         {
            _jo = ReadJsonString(name);
            CanWrite = false;
         }

         CanRead = true;
      }

      public void Dispose()
      {
         // nothing to dispose.
      }

      public string Name => "json";

      public bool CanRead { get; }

      public bool CanWrite { get; }

      public string Read(string key)
      {
         if (key == null || _jo == null) return null;

         bool isLength = OptionPath.TryStripLength(key, out key);

         string path = "$." + key;

         JToken valueToken = _jo.SelectToken(path);

         if(isLength)
         {
            if(valueToken is JArray arrayToken)
            {
               return arrayToken.Count.ToString();
            }

            return null;
         }

         return GetStringValue(valueToken);
      }

      private string GetStringValue(JToken token)
      {
         if (token == null) return null;

         return token.Value<string>();
      }

      public void Write(string key, string value)
      {
         if (key == null || _jo == null || !CanWrite) return;

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

      private static JObject ReadJsonFile(string fileName)
      {
         if(File.Exists(fileName))
         {
            string json = File.ReadAllText(fileName);
            return JObject.Parse(json);
         }

         return new JObject();
      }

      private static JObject ReadJsonString(string jsonString)
      {
         return JObject.Parse(jsonString);
      }

      private void WriteJsonFile()
      {
         if (_jo == null) return;

         // Create temporary file with new data.
         string json = _jo.ToString(Formatting.Indented);
         string tempPath = Path.GetTempPath() + Guid.NewGuid().ToString() + ".tmp";
         byte[] data = Encoding.UTF8.GetBytes(json);
         using (FileStream tempFile = File.Create(tempPath, 4096, FileOptions.WriteThrough))
            tempFile.Write(data, 0, data.Length);

         // Create backup of the destination file with old data.
         // Since backup file is required, an exception allowed.
         string backupPath = _pathName + ".backup";
         File.Copy(_pathName, backupPath, true);

         // Try to overwrite destination file (_pathName) with new one (tempPath)
         // and delete backup with temporary file if successful.
         // Otherwise, restore backup data and remove unneccessary files.
         try
         {
            File.Copy(tempPath, _pathName, true);
         }
         catch (Exception)
         {
            File.Copy(backupPath, _pathName, true);
         }
         finally
         {
            File.Delete(backupPath);
            File.Delete(tempPath);
         }
      }
   }
}