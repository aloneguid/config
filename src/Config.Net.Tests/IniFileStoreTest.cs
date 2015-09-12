using System.Collections.Generic;
using System.IO;
using System.Linq;
using Config.Net.Stores;
using NUnit.Framework;

namespace Config.Net.Tests
{
   [TestFixture]
   public class IniFileStoreTest : AbstractTestFixture
   {
      private string _testFile;
      private IniFileConfigStore _store;

      [SetUp]
      public void CreateTestFile()
      {
         string dir = BuildDir.FullName;
         string src = Path.Combine(dir, "TestData", "example.ini");
         _testFile = Path.Combine(dir, "test.ini");
         File.Copy(src, _testFile, true);

         _store = new IniFileConfigStore(_testFile);
      }

      [TearDown]
      public void DeleteTestFile()
      {
         _store.Dispose();

         File.Delete(_testFile);
      }

      [Test]
      public void ReadSectionlessTest()
      {
         string value0 = _store.Read("key0");
         Assert.AreEqual("value0", value0);
      }

      [Test]
      public void ReadSection1Test()
      {
         string value1 = _store.Read("key1");
         Assert.AreEqual("value1", value1);
      }


      [Test]
      public void ReadCommentedKeyTest()
      {
         string value = _store.Read("key5");
         Assert.AreEqual(null, value);
      }

      [Test]
      public void ReadCommentedKey2Test()
      {
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
         Assert.IsTrue(_store.CanWrite);

         _store.Write(key, value);

         Assert.AreEqual(value, _store.Read(key));
      }

      [Test]
      public void Write_ReplacesValue_ReadsBackCorrectly()
      {
         const string key = "key7";
         const string value = "changedvalue7";

         Assert.IsTrue(_store.CanWrite);

         _store.Write(key, value);

         Assert.AreEqual(value, _store.Read(key));
      }

      [Test]
      public void Read_CommentedKeyValue_ReturnsNull()
      {
         const string key = "key5";
         
         Assert.AreEqual(null, _store.Read(key));
      }

      [Test]
      public void Write_ReplacesValue_FileFormatIsSameAndReadsBackCorrectly()
      {
         const string key = "key7";
         const string value = "changedvalue7";
         List<string> fileContentsBeforeChange = File.ReadLines(_testFile).ToList();
         
         _store.Write(key, value);
         List<string> fileContentsAfterChange = File.ReadLines(_testFile).ToList();
         
         Assert.AreEqual(1, fileContentsAfterChange.Except(fileContentsBeforeChange).Count());
      }

      [Test]
      public void Write_AddsValue_FileFormatIsSameAndReadsBackCorrectly()
      {
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
         Assert.IsTrue(_store.CanWrite);

         _store.Write("key9", "value=9");

         Assert.AreEqual("value=9", _store.Read("key9"));
      }

      [Test]
      public void Write_AddKeyAndValueWithEqualToSignDelimiter_ShouldWriteAndReadCorrectly()
      {
         Assert.IsTrue(_store.CanWrite);

         _store.Write("key=10", "value=10");

         Assert.AreEqual("value=10", _store.Read("key=10"));
      }
   }
}
