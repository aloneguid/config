using System;
using Config.Net.Stores;
using Xunit;

namespace Config.Net.Tests
{
   public class AllSettings : SettingsContainer
   {
      public readonly Option<string> AuthClientId = new Option<string>();

      public readonly Option<string> AuthClientSecret = new Option<string>();

      protected override void OnConfigure(IConfigConfiguration configuration)
      {
         configuration.CacheTimeout = TimeSpan.FromMinutes(1);

         configuration.UseAzureKeyVault();
         configuration.UseAzureTable("storage_account_name", "storage_key", "table_name", "application_name");
      }
   }

   public class SettingsContainerTest
   {
      private IConfigStore _store;

      public SettingsContainerTest()
      {
         _store = new InMemoryConfigStore();
      }

      class MyContainer : SettingsContainer
      {
         #region crap
         private IConfigStore _store;

         public MyContainer(IConfigStore store) : base("MyApp")
         {
            _store = store;
         }
         #endregion

         public readonly Option<TimeSpan> StrongSpan = new Option<TimeSpan>(TimeSpan.FromDays(1));

         public readonly Option<int> Timeout = new Option<int>(2);

         public readonly Option<int> NoInitTimeout;

         protected override void OnConfigure(IConfigConfiguration configuration)
         {
            configuration.AddStore(_store);
         }
      }

      [Fact]
      public void Read_IntegerTimeout_Reads()
      {
         var c = new MyContainer(_store);

         int timeout = c.Timeout;

         Assert.Equal(2, timeout);
      }

      [Fact]
      public void Read_NoInit_Acceptable()
      {
         var c = new MyContainer(_store);

         Assert.Equal("MyApp.NoInitTimeout", c.NoInitTimeout.Name);
      }

      public void Demo()
      {
         var c = new AllSettings();

         string clientId = c.AuthClientId;
         string clientSecret = c.AuthClientSecret;

         c.AuthClientId.Write("new value");
      }

      private class LambdaModule
      {
         
      }
   }
}
