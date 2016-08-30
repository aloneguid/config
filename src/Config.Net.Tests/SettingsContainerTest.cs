using System;
using Config.Net.Stores;
using NUnit.Framework;

namespace Config.Net.Tests
{
   [TestFixture]
   public class SettingsContainerTest
   {
      private IConfigStore _store;

      [SetUp]
      public void SetUp()
      {
         _store = new InMemoryConfigStore();
      }

      /*class MySection : SettingsSection
      {
         public MySection() : base("MySection1")
         {
         }

         public Option<int> IntOption = new Option<int>()
      }*/

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

      [Test]
      public void Read_IntegerTimeout_Reads()
      {
         var c = new MyContainer(_store);

         int timeout = c.Timeout;

         Assert.AreEqual(2, timeout);
      }

      private class LambdaModule
      {
         
      }
   }
}
