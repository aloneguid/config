using System;

namespace Config.Net.Tests
{
   public class TestSettings : SettingsContainer
   {
      public readonly Option<string> AzureStorageName = new Option<string>("Azure.Storage.Name", null);

      public readonly Option<string> AzureStorageKey = new Option<string>("Azure.Storage.Key", null);

      public readonly Option<Uri> AzureKeyVaultUri = new Option<Uri>("Azure.KeyVault.Url", null);

      public readonly Option<string> AzureKeyVaultClientId = new Option<string>("Azure.KeyVault.Aad.ClientId", null);

      public readonly Option<string> AzureKeyVaultSecret = new Option<string>("Azure.KeyVault.Aad.ClientSecret", null);

      protected override void OnConfigure(IConfigConfiguration configuration)
      {
         configuration.UseIniFile("c:\\tmp\\integration-tests.ini");
         configuration.UseEnvironmentVariables();
      }
   }

   public static class Settings
   {
      private static TestSettings _settings = new TestSettings();

      public static TestSettings Test => _settings;
   }
}
