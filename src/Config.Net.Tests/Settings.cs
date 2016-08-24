using System;

namespace Config.Net.Tests
{
   class TestSettings : SettingsContainer
   {
      public Option<string> AzureStorageName = new Option<string>("Azure.Storage.Name");

      public Option<string> AzureStorageKey = new Option<string>("Azure.Storage.Key");

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
