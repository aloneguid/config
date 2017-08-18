using Storage.Net;
using Storage.Net.Blob;
using System;
using System.Collections.Generic;
using System.Text;

namespace Config.Net.Integration.Storage.Net
{
   class BlobConfigStore : IConfigStore
   {
      private readonly IBlobStorage _blobs;

      public BlobConfigStore(IBlobStorage blobs, bool canWrite = true)
      {
         CanWrite = canWrite;
         _blobs = blobs ?? throw new ArgumentNullException(nameof(blobs));
      }

      public string Name => "Storage.Net Blobs";

      public bool CanRead => true;

      public bool CanWrite { get; set; }

      public void Dispose()
      {
      }

      public string Read(string key)
      {
         if (key == null) return null;

         try
         {
            return _blobs.ReadText(key);
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
            _blobs.Delete(key);
         }
         else
         {
            _blobs.WriteText(key, value);
         }
      }
   }
}
