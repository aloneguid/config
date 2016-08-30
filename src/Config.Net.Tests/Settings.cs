using System;

namespace Config.Net.Tests
{
   class TestSettings : SettingsContainer
   {
      public readonly Option<string> AzureStorageName = new Option<string>("Azure.Storage.Name", null);

      public readonly Option<string> AzureStorageKey = new Option<string>("Azure.Storage.Key", null);

      protected override void OnConfigure(IConfigConfiguration configuration)
      {
         configuration.UseIniFile("c:\\tmp\\integration-tests.ini");
         configuration.UseEnvironmentVariables();
      }
   }

   static class Settings
   {
      private static TestSettings _settings = new TestSettings();

      public static TestSettings Test => _settings;
   }
}
