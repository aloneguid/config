using System.Collections.Generic;
using System.IO;
using System.Linq;
using Config.Net.Azure;
using Config.Net.Stores;
using NUnit.Framework;

namespace Config.Net.Tests
{
   [TestFixture("ini")]
   [TestFixture("azTable")]
   public class AllStoresTest : AbstractTestFixture
   {
      private IConfigStore _store;
      private readonly string _storeName;
      private string _testFile;

      public AllStoresTest(string storeName)
      {
         _storeName = storeName;
      }


      [SetUp]
      public void CreateStore()
      {
         switch(_storeName)
         {
            case "ini":
               string dir = BuildDir.FullName;
               string src = Path.Combine(dir, "TestData", "example.ini");
               _testFile = Path.Combine(dir, "test.ini");
               File.Copy(src, _testFile, true);
               _store = new IniFileConfigStore(_testFile);
               break;
            case "azTable":
               _store = new AzureTableConfigStore("", "", "configurationtest", "confignettests");
               break;
         }
      }

      [TearDown]
      public void DisposeStore()
      {
         _store.Dispose();
      }

      [Test]
      public void Ini_ReadSectionlessTest()
      {
         if(_storeName != "ini") Assert.Ignore();

         string value0 = _store.Read("key0");
         Assert.AreEqual("value0", value0);
      }

      [Test]
      public void Ini_ReadSection1Test()
      {
         if(_storeName != "ini") Assert.Ignore();

         string value1 = _store.Read("key1");
         Assert.AreEqual("value1", value1);
      }

      [Test]
      public void Ini_ReadCommentedKeyTest()
      {
         if(_storeName != "ini") Assert.Ignore();

         string value = _store.Read("key5");
         Assert.AreEqual(null, value);
      }

      [Test]
      public void Ini_ReadCommentedKey2Test()
      {
         if(_storeName != "ini") Assert.Ignore();
         string value = _store.Read("key7");
         Assert.AreEqual("value7", value);
      }

      [Test]
      [TestCase("testkey", "testvalue")]
      [TestCase("testkey1", "34567")]
      [TestCase("testkey2", "HOMER,BART,LISA,MARGE,MAGGI")]
      [TestCase("testkey3", null)]
      public void Write_WritesKeyValue_ReadsBackCorrectly(string key, string value)
      {
         IgnoreNonWriteable();

         _store.Write(key, value);

         Assert.AreEqual(value, _store.Read(key));
      }

      [Test]
      public void Write_ReplacesValue_ReadsBackCorrectly()
      {
         IgnoreNonWriteable();

         const string key = "key7";
         const string value = "changedvalue7";

         Assert.IsTrue(_store.CanWrite);

         _store.Write(key, value);

         Assert.AreEqual(value, _store.Read(key));
      }

      [Test]
      public void IniRead_CommentedKeyValue_ReturnsNull()
      {
         if(_storeName != "ini") Assert.Ignore();

         const string key = "key5";
         
         Assert.AreEqual(null, _store.Read(key));
      }

      [Test]
      public void IniWrite_ReplacesValue_FileFormatIsSameAndReadsBackCorrectly()
      {
         if(_storeName != "ini") Assert.Ignore();
         const string key = "key7";
         const string value = "changedvalue7";
         List<string> fileContentsBeforeChange = File.ReadLines(_testFile).ToList();
         
         _store.Write(key, value);
         List<string> fileContentsAfterChange = File.ReadLines(_testFile).ToList();
         
         Assert.AreEqual(1, fileContentsAfterChange.Except(fileContentsBeforeChange).Count());
      }

      [Test]
      public void IniWrite_AddsValue_FileFormatIsSameAndReadsBackCorrectly()
      {
         if(_storeName != "ini") Assert.Ignore();

         const string key = "key8";
         const string value = "value8";
         List<string> fileContentsBeforeChange = File.ReadLines(_testFile).ToList();

         _store.Write(key, value);
         List<string> fileContentsAfterChange = File.ReadLines(_testFile).ToList();
         
         Assert.AreEqual("key8=value8", fileContentsAfterChange.Except(fileContentsBeforeChange).First());
      }

      [Test]
      public void Write_AddValueWithEqualToSignDelimiter_ShouldWriteAndReadCorrectly()
      {
         IgnoreNonWriteable();

         _store.Write("key9", "value=9");

         Assert.AreEqual("value=9", _store.Read("key9"));
      }

      [Test]
      public void Write_AddKeyAndValueWithEqualToSignDelimiter_ShouldWriteAndReadCorrectly()
      {
         IgnoreNonWriteable();

         _store.Write("key=10", "value=10");

         Assert.AreEqual("value=10", _store.Read("key=10"));
      }

      private void IgnoreNonWriteable()
      {
         if(!_store.CanWrite) Assert.Ignore("ignored as store is not writeable");
      }
   }
}
