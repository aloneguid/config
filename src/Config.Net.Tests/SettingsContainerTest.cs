using System;
using Config.Net.Stores;
using Xunit;

namespace Config.Net.Tests
{
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

         //public static readonly Option<int> StaticTimeout = new Option<int>(1);

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

      private class LambdaModule
      {
         
      }
   }
}
