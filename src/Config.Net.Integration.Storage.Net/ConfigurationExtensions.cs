using Storage.Net.Blob;

namespace Config.Net.Integration.Storage.Net
{
   public static class ConfigurationExtensions
   {
      public static ConfigurationBuilder<TInterface> UseStorageNetBlobs<TInterface>(this ConfigurationBuilder<TInterface> builder, IBlobStorageProvider blobs) where TInterface : class
      {
         IConfigStore store = new BlobConfigStore(blobs);

         builder.UseConfigStore(store);

         return builder;
      }
   }
}