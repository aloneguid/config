using Microsoft.WindowsAzure.Storage.Table;

namespace Config.Net.Azure.Model
{
   /// <summary>
   /// Used to store values in <see cref="AzureTableConfigStore"/>
   /// </summary>
   class AzureTableRecord : TableEntity
   {
      public string Value { get; set; }
   }
}
