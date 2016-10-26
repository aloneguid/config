#if NETFULL
using System;
using System.Collections.Generic;
using System.Linq;
using Config.Net.Azure.Model;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;

namespace Config.Net.Azure
{
   class AzureTableConfigStore : IConfigStore
   {
      private readonly CloudTable _table;
      private readonly string _appName;

      public AzureTableConfigStore(string accountName, string storageKey, string tableName, string appName)
      {
         if(accountName == null) throw new ArgumentNullException(nameof(accountName));
         if(storageKey == null) throw new ArgumentNullException(nameof(storageKey));
         if(tableName == null) throw new ArgumentNullException(nameof(tableName));
         if(appName == null) throw new ArgumentNullException(nameof(appName));

         var account = new CloudStorageAccount(new StorageCredentials(accountName, storageKey), true);
         var client = account.CreateCloudTableClient();
         _table = client.GetTableReference(tableName);
         _table.CreateIfNotExists();
         _appName = appName;
      }

      public string Name => "Azure Table Config";

      public bool CanRead { get; } = true;

      public bool CanWrite { get; } = true;

      public string Read(string key)
      {
         if (key == null) return null;

         var filter = TableQuery.CombineFilters(
            TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, _appName),
            TableOperators.And,
            TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, key));
         var query = new TableQuery<AzureTableRecord>().Where(filter);
         IEnumerable<AzureTableRecord> records = _table.ExecuteQuery(query);
         AzureTableRecord record = records?.FirstOrDefault();
         return record?.Value;
      }

      public void Write(string key, string value)
      {
         var batch = new TableBatchOperation();
         var record = new AzureTableRecord
         {
            PartitionKey = _appName,
            RowKey = key,
            ETag = "*"
         };

         if(value != null)
         {
            record.Value = value;
            batch.InsertOrMerge(record);
         }
         else
         {
            batch.Delete(record);
         }

         try
         {
            _table.ExecuteBatch(batch);
         }
         catch(StorageException)
         {
            //not ideal check. If value is null (Delete operation) and record doesn't exist
            //I ignore the exception as it simply says that entity doesn't exist which is fine.
            //I don't know how to check for the specific error code.
            if(value != null) throw;
         }
      }

      public void Dispose()
      {
         //nothing to dispose
      }
   }
}
#endif