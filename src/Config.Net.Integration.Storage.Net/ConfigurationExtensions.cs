using Storage.Net.Blob;

namespace Config.Net.Integration.Storage.Net
{
   public static class ConfigurationExtensions
   {
      public static IConfigConfiguration UseStorageNetBlobs(this IConfigConfiguration configuration, IBlobStorageProvider blobs)
      {
         IConfigStore store = new BlobConfigStore(blobs);

         configuration.AddStore(store);

         return configuration;
      }
   }
}