using System;
using System.IO;
using System.Threading;
using Config.Net.Stores;
using NUnit.Framework;

namespace Config.Net.Tests.Stores
{
   [TestFixture]
   public class IniFileConfigStoreTest : AbstractTestFixture
   {
      private string _testFilePath;
      private IniFileConfigStore _store;

      [SetUp]
      public void SetUp()
      {
         //get back clean file
         string src = Path.Combine(BuildDir.FullName, "TestData", "example.ini");
         _testFilePath = Path.Combine(TestDir.FullName, "example.ini");
         File.Copy(src, _testFilePath, true);

         //create the store
         _store = new IniFileConfigStore(_testFilePath);
      }

      [TearDown]
      public void TearDown()
      {
         _store.Dispose();
      }

      [Test]
      public void Read_CleanFile_CorrectValue()
      {
         Assert.AreEqual("value1", _store.Read("key1"));
      }

      [Test]
      public void Read_FileDoesNotExist_DoesNotFail()
      {
         var store = new IniFileConfigStore($"c:\\{Guid.NewGuid()}.ini");
      }

      [Test]
      public void ValuesChange_RewriteFileWithNewValues_ReadsNewValues()
      {
         File.WriteAllText(_testFilePath, "keyV=valueV");

         //Assert.AreEqual("valueV", _store.Read("keyV")); //this may fail immediately because it takes time for FSW to pick up the changes

         Thread.Sleep(TimeSpan.FromSeconds(1));

         Assert.AreEqual("valueV", _store.Read("keyV"));
      }
   }
}
