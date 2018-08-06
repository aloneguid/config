using Storage.Net.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace Config.Net.Integration.Storage.Net
{
   //todo: not ready for use yet
   class TableConfigStore : IConfigStore
   {
      private readonly ITableStorage _tableStorage;
      private readonly string _tableName;
      private readonly string _partitionKey;

      public TableConfigStore(ITableStorage tableStorage, string tableName, string partitionKey)
      {
         _tableStorage = tableStorage ?? throw new ArgumentNullException(nameof(tableStorage));
         _tableName = tableName ?? throw new ArgumentNullException(nameof(tableName));
         _partitionKey = partitionKey ?? throw new ArgumentNullException(nameof(partitionKey));
      }

      public string Name => "Storage.Net Tables";

      public bool CanRead => true;

      public bool CanWrite => true;

      public void Dispose()
      {
         _tableStorage.Dispose();
      }

      public string Read(string key)
      {
         TableRow row = _tableStorage.GetAsync(_tableName, _partitionKey, key).Result;
         if (row == null) return null;

         return row["value"];
      }

      public void Write(string key, string value)
      {
         var rowId = new TableRowId(_partitionKey, key);

         if (value == null)
         {
            _tableStorage.DeleteAsync(_tableName, new[] { rowId }).Wait();
         }
         else
         {
            var row = new TableRow(rowId);
            row["value"] = value;
            _tableStorage.InsertOrReplaceAsync(_tableName, new[] { row } ).Wait();
         }
      }
   }
}
