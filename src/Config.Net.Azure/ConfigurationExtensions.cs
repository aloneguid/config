using Config.Net.Azure;

namespace Config.Net
{
   /// <summary>
   /// Configuration extensions
   /// </summary>
   public static class ConfigurationExtensions
   {
      /// <summary>
      /// Use azure configuration manager
      /// </summary>
      public static IConfigConfiguration UseAzureConfigStore(this IConfigConfiguration configuration)
      {
         configuration.AddStore(new AzureConfigStore());
         return configuration;
      }

      /// <summary>
      /// Use azure tables
      /// </summary>
      public static IConfigConfiguration UseAzureTable(this IConfigConfiguration configuration,
         string accountName, string storageKey, string tableName, string appName)
      {
         configuration.AddStore(new AzureTableConfigStore(accountName, storageKey, tableName, appName));
         return configuration;
      }

      public static IConfigConfiguration UseAzureKeyVault(this IConfigConfiguration configuration)
      {


         return configuration;
      }
   }
}
