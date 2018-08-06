using Config.Net.Integration.Storage.Net;
using Storage.Net.Blob;

namespace Config.Net
{
   public static class ConfigurationExtensions
   {
      /// <summary>
      /// Use <see cref="IBlobStorage"/> as underlying configuration storage. Every blob will correspond to a key storing it's value as the content.
      /// </summary>
      public static ConfigurationBuilder<TInterface> UseStorageNetBlobs<TInterface>(this ConfigurationBuilder<TInterface> builder, IBlobStorage blobStorage) where TInterface : class
      {
         IConfigStore store = new BlobConfigStore(blobStorage);

         builder.UseConfigStore(store);

         return builder;
      }
   }
}