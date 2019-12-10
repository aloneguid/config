using Storage.Net;
using Storage.Net.Blobs;
using System;

namespace Config.Net.Integration.Storage.Net
{
   class BlobConfigStore : IConfigStore
   {
      private readonly IBlobStorage _blobs;

      public BlobConfigStore(IBlobStorage blobs)
      {
         _blobs = blobs ?? throw new ArgumentNullException(nameof(blobs));
      }

      public string Name => "Storage.Net Blobs";

      public bool CanRead => true;

      public bool CanWrite => true;

      public void Dispose()
      {
      }

      public string Read(string key)
      {
         if (key == null) return null;

         try
         {
            return _blobs.ReadTextAsync(key).Result;
         }
         catch (StorageException ex) when (ex.ErrorCode == ErrorCode.NotFound)
         {
            return null;
         }
      }

      public void Write(string key, string value)
      {
         if (value == null)
         {
            _blobs.DeleteAsync(key).Wait();
         }
         else
         {
            _blobs.WriteTextAsync(key, value).Wait();
         }
      }
   }
}
