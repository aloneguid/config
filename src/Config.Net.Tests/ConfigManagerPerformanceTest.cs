using Moq;
using NUnit.Framework;
using System;

namespace Config.Net.Tests
{
   [TestFixture]
   public class ConfigManagerPerformanceTest
   {
      private Mock<IConfigStore> _store;
      private static readonly Setting<int> IntSetting = new Setting<int>("int", 4);

      [SetUp]
      public void SetUp()
      {
         _store = new Mock<IConfigStore>();
         _store.Setup(s => s.CanRead).Returns(true);

         Cfg.Configuration.RemoveAllStores();
         Cfg.Configuration.AddStore(_store.Object);
         Cfg.Configuration.CacheTimeout = TimeSpan.Zero;
      }

      [Test]
      public void ReadTwice_NoCaching_TwoCallsToStore()
      {
         _store.Setup(s => s.Read(It.IsAny<string>())).Returns((string)null);

         Cfg.Read(IntSetting);
         Cfg.Read(IntSetting);

         _store.Verify(s => s.Read(IntSetting.Name), Times.Exactly(2));
      }
   }
}
