using System;
using Config.Net.Json.Stores;
using Newtonsoft.Json.Linq;

namespace Config.Net
{
   /// <summary>
   /// Configuration extensions
   /// </summary>
   public static class ConfigurationExtensions
   {
      /// <summary>
      /// Uses JSON file as a builder storage.
      /// </summary>
      /// <param name="builder">Configuration object.</param>
      /// <param name="jsonFilePath">Full path to json storage file.</param>
      /// <returns>Changed builder.</returns>
      /// <remarks>Storage file does not have to exist, however it will be created as soon as first write performed.</remarks>
      public static ConfigurationBuilder<TInterface> UseJsonFile<TInterface>(this ConfigurationBuilder<TInterface> builder, string jsonFilePath) where TInterface : class
      {
         builder.UseConfigStore(new JsonFileConfigStore(jsonFilePath, true));
         return builder;
      }

      /// <summary>
      /// Uses JSON file as a builder storage.
      /// </summary>
      /// <param name="builder">Configuration object.</param>
      /// <param name="jsonString">Json document.</param>
      /// <returns>Changed builder.</returns>
      /// <remarks>Storage file does not have to exist, however it will be created as soon as first write performed.</remarks>
      public static ConfigurationBuilder<TInterface> UseJsonString<TInterface>(this ConfigurationBuilder<TInterface> builder, string jsonString) where TInterface : class
      {
         builder.UseConfigStore(new JsonFileConfigStore(jsonString, false));
         return builder;
      }

      /// <summary>
      /// Uses JSON file (appsettings.json) as a builder storage. Read-only.
      /// </summary>
      /// <param name="builder">Configuration object.</param>
      /// <param name="jsonFilePath">Relative or full path to json file.</param>
      /// <param name="settings">Settings used when merging JSON.</param>
      /// <returns>Changed builder.</returns>
      /// <remarks>When using default <code>UseJsonConfig()</code> appsettings.json must exist otherwise empty storage will be returned.</remarks>
      public static ConfigurationBuilder<TInterface> UseJsonConfig<TInterface>(this ConfigurationBuilder<TInterface> builder, string jsonFilePath = "appsettings.json", Action<JsonMergeSettings> settings = null) where TInterface : class
      {
         JsonMergeSettings jsonMergeSettings = EnvironmentFileBuilder.DefaultJsonMergeSettings;
         settings?.Invoke(jsonMergeSettings);
         builder.UseConfigStore(new EnvironmentFileBuilder().Build(jsonFilePath, jsonMergeSettings));
         return builder;
      }
   }
}
