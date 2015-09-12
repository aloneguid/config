using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;

namespace Config.Net.Azure
{
   public class AzureTableConfigStore : IConfigStore
   {
      private readonly CloudTableClient _client;

      private readonly string _tableName;
      private readonly string _appName;
      private readonly object _readLock = new object();
      private DateTime _lastRead = DateTime.MinValue;
      private readonly ConcurrentDictionary<string, string> _cache = new ConcurrentDictionary<string, string>();

      public AzureTableConfigStore(string accountName, string storageKey, string tableName, string appName)
      {
         if(accountName == null) throw new ArgumentNullException(nameof(accountName));
         if(storageKey == null) throw new ArgumentNullException(nameof(storageKey));
         if(tableName == null) throw new ArgumentNullException(nameof(tableName));
         if(appName == null) throw new ArgumentNullException(nameof(appName));

         var account = new CloudStorageAccount(new StorageCredentials(accountName, storageKey), true);
         _client = account.CreateCloudTableClient();

         _tableName = tableName;
         _appName = appName;
      }

      public string Name => "Azure Table Config";

      public bool CanRead { get; } = true;

      public bool CanWrite { get; } = true;

      public string Read(string key)
      {
         CheckCache();

         string result;
         _cache.TryGetValue(key, out result);
         return result;
      }

      public void Write(string key, string value)
      {
         if (value != null)
         {
            var row = new TableRow(_appName, key);
            row["value"] = value;
            _tableStorage.Merge(_tableName, row);
         }
         else
         {
            _tableStorage.Delete(_tableName, new TableRowId(_appName, key));
         }

         InvalidateCache();
      }

      private void InvalidateCache()
      {
         _lastRead = DateTime.MinValue;
      }

      private void CheckCache()
      {
         lock (_readLock)
         {
            if (DateTime.UtcNow - _lastRead < TimeSpan.FromHours(1)) return;

            _cache.Clear();
            _lastRead = DateTime.UtcNow;
            IEnumerable<TableRow> rows = _tableStorage.Get(_tableName, _appName);
            if (rows != null)
            {
               foreach (TableRow row in rows)
               {
                  _cache[row.RowKey] = row["value"];
               }
            }
         }
      }

      public void Dispose()
      {
      }
   }
}
