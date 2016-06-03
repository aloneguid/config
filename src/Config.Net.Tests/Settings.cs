namespace Config.Net.Tests
{
   static class Settings
   {
      public static readonly Setting<string> AzureStorageName = new Setting<string>("Azure.Storage.Name", null);

      public static readonly Setting<string> AzureStorageKey = new Setting<string>("Azure.Storage.Key", null);
   }
}
