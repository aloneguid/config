using System.IO;
using Config.Net.Azure;
using Config.Net.Stores;
using NUnit.Framework;

namespace Config.Net.Tests
{
   [TestFixture("ini")]
   [TestFixture("azTable")]
   [TestFixture("inmemory")]
   [TestFixture("appconfig")]
   [TestFixture("env")]
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
         Cfg.Configuration.RemoveAllStores();
         Cfg.Configuration.AddStore(new IniFileConfigStore("c:\\tmp\\integration-tests.ini"));
         Cfg.Configuration.AddStore(new EnvironmentVariablesStore());

         switch (_storeName)
         {
            case "ini":
               string dir = BuildDir.FullName;
               string src = Path.Combine(dir, "TestData", "example.ini");
               _testFile = Path.Combine(dir, "test.ini");
               File.Copy(src, _testFile, true);
               _store = new IniFileConfigStore(_testFile);
               break;
            case "azTable":
               _store = new AzureTableConfigStore(
                  Settings.AzureStorageName,
                  Settings.AzureStorageKey,
                  "configurationtest", "confignettests");
               break;
            case "inmemory":
               _store = new InMemoryConfigStore();
               break;
            case "appconfig":
               _store = new AppConfigStore();
               break;
            case "env":
               _store = new EnvironmentVariablesStore();
               break;
         }
      }

      [TearDown]
      public void DisposeStore()
      {
         _store.Dispose();
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

         _store.Write(key, value);

         Assert.AreEqual(value, _store.Read(key));
      }

      [Test]
      public void Write_AddValueWithEqualToSignDelimiter_ShouldWriteAndReadCorrectly()
      {
         IgnoreNonWriteable();

         _store.Write("key9", "value=9");

         Assert.AreEqual("value=9", _store.Read("key9"));
      }

      [Test, Ignore("this needs extra escaping support")]
      public void Write_AddKeyAndValueWithEqualToSignDelimiter_ShouldWriteAndReadCorrectly()
      {
         IgnoreNonWriteable();

         _store.Write("key=10", "value=10");

         //re-initialise the store
         CreateStore();

         Assert.AreEqual("value=10", _store.Read("key=10"));
      }

      private void IgnoreNonWriteable()
      {
         if(!_store.CanWrite) Assert.Ignore("ignored as store is not writeable");
      }
   }
}
