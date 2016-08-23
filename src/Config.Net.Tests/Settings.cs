namespace Config.Net.Tests
{
   static class Settings
   {
      public static readonly Option<string> AzureStorageName = new Option<string>("Azure.Storage.Name", null);

      public static readonly Option<string> AzureStorageKey = new Option<string>("Azure.Storage.Key", null);
   }
}
