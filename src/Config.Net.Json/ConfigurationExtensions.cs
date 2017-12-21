using Config.Net.Json.Stores;

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
         builder.UseConfigStore(new JsonFileConfigStore(jsonFilePath));
         return builder;
      }

   }
}
