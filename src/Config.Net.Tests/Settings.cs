using System;
using System.Collections.Generic;

namespace Config.Net.Tests
{
   public class TestSettings : SettingsContainer
   {
      public Option<string> AzureStorageName { get; } = new Option<string>("Azure.Storage.Name", null);

      public Option<string> AzureStorageKey { get; } = new Option<string>("Azure.Storage.Key", null);

      public Option<Uri> AzureKeyVaultUri { get; } = new Option<Uri>("Azure.KeyVault.Url", null);

      public Option<string> AzureKeyVaultClientId { get; } = new Option<string>("Azure.KeyVault.Aad.ClientId", null);

      public Option<string> AzureKeyVaultSecret { get; } = new Option<string>("Azure.KeyVault.Aad.ClientSecret", null);

      protected override void OnConfigure(IConfigConfiguration configuration)
      {
         configuration.UseIniFile("c:\\tmp\\integration-tests.ini");
      }
   }

   public static class Settings
   {
      private static TestSettings _settings = new TestSettings();

      public static TestSettings Test => _settings;
   }
}
