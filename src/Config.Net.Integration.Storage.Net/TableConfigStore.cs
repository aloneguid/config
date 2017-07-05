using Storage.Net.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace Config.Net.Integration.Storage.Net
{
   class TableConfigStore : IConfigStore
   {
      private readonly ITableStorage _tableStorage;
      private readonly string _tableName;
      private readonly string _partitionKey;

      public TableConfigStore(ITableStorage tableStorage, string tableName, string partitionKey)
      {
         _tableStorage = tableStorage;
         _tableName = tableName;
         _partitionKey = partitionKey;
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
         TableRow row = _tableStorage.Get(_tableName, _partitionKey, key);
         if (row == null) return null;

         return row["value"];
      }

      public void Write(string key, string value)
      {
         var rowId = new TableRowId(_partitionKey, key);

         if (value == null)
         {
            _tableStorage.Delete(_tableName, rowId);
         }
         else
         {
            var row = new TableRow(rowId);
            row["value"] = value;
            _tableStorage.InsertOrReplace(_tableName, row);
         }
      }
   }
}
