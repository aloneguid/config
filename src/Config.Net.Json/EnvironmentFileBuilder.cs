using System;
using System.IO;
using Config.Net.Json.Stores;
using Newtonsoft.Json.Linq;

namespace Config.Net
{
   public class EnvironmentFileBuilder
   {
      public const string EnvironmentKey = "APP_ENV";

      public static JsonMergeSettings DefaultJsonMergeSettings => new JsonMergeSettings()
      {
         MergeArrayHandling = MergeArrayHandling.Merge,
         MergeNullValueHandling = MergeNullValueHandling.Ignore
      };

      public IConfigStore Build(string jsonFilePath)
      {
         return Build(jsonFilePath, DefaultJsonMergeSettings);
      }

      public IConfigStore Build(string jsonFilePath, JsonMergeSettings jsonMergeSettings)
      {
         if (jsonFilePath == null) throw new ArgumentNullException(nameof(jsonFilePath));

         string environment = Environment.GetEnvironmentVariable(EnvironmentKey);

         if (string.IsNullOrWhiteSpace(environment) || environment.Equals("Production"))
         {
            return new JsonFileConfigStore(ReadJsonFile(jsonFilePath).ToString(), false);
         }

         string mergedJsonFile = MergeJsonFile(jsonFilePath, GetEnvironmentJsonFile(jsonFilePath, environment), jsonMergeSettings);

         return new JsonFileConfigStore(mergedJsonFile, false);
      }

      private string GetEnvironmentJsonFile(string jsonFilePath, string environment)
      {
         int lastDotIndex = jsonFilePath.LastIndexOf(".", StringComparison.CurrentCultureIgnoreCase);
         string environmentJsonFilePath = jsonFilePath.Insert(lastDotIndex, $".{environment}");

         return environmentJsonFilePath;
      }

      private string MergeJsonFile(string firstFilePath, string secondFilePath, JsonMergeSettings jsonMergeSettings)
      {
         JObject first = ReadJsonFile(firstFilePath);
         JObject second = ReadJsonFile(secondFilePath);

         first.Merge(second, jsonMergeSettings);
         string merged = first.ToString();

         return merged;
      }

      private static JObject ReadJsonFile(string fileName)
      {
         if (!File.Exists(fileName)) return new JObject();
         string json = File.ReadAllText(fileName);

         return JObject.Parse(json);
      }
   }
}