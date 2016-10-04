using System;
using System.IO;
using Config.Net.Azure;
using Config.Net.Stores;
using Xunit;

namespace Config.Net.Tests
{
   public class IniFileTest : AllStoresTest
   {
      public IniFileTest() : base("ini") { }
   }

   public class AzureTablesTest : AllStoresTest
   {
      public AzureTablesTest() : base("azTable") { }
   }

   public class InMemoryTest : AllStoresTest
   {
      public InMemoryTest() : base("inmemory") { }
   }

   public class AppConfigTest : AllStoresTest
   {
      public AppConfigTest() : base("appconfig") { }
   }

   public class EnvironmentVariablesTest : AllStoresTest
   {
      public EnvironmentVariablesTest() : base("env") { }
   }

   public class AzureKeyVaultTest : AllStoresTest
   {
      public AzureKeyVaultTest() : base("azKeyVault") { }
   }

   /// <summary>
   /// Tests all stores for consistent behavior
   /// </summary>
   public abstract class AllStoresTest : AbstractTestFixture, IDisposable
   {
      private IConfigStore _store;
      private readonly string _storeName;
      private string _testFile;
      private TestSettings _settings = new TestSettings();

      public AllStoresTest(string storeName)
      {
         _storeName = storeName;
     
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
                  _settings.AzureStorageName,
                  _settings.AzureStorageKey,
                  "configurationtest", "confignettests");
               break;
            case "azKeyVault":
               _store = new AzureKeyVaultConfigStore(
                  _settings.AzureKeyVaultUri,
                  _settings.AzureKeyVaultClientId,
                  _settings.AzureKeyVaultSecret);
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

      [Theory]
      [InlineData("testkey", "testvalue")]
      [InlineData("testkey1", "34567")]
      [InlineData("testkey2", "HOMER,BART,LISA,MARGE,MAGGI")]
      [InlineData("testkey3", null)]
      public void Write_WritesKeyValue_ReadsBackCorrectly(string key, string value)
      {
         if(!_store.CanWrite) return;

         _store.Write(key, value);

         Assert.Equal(value, _store.Read(key));
      }

      [Fact]
      public void Write_ReplacesValue_ReadsBackCorrectly()
      {
         if(!_store.CanWrite) return;

         const string key = "key7";
         const string value = "changedvalue7";

         _store.Write(key, value);

         Assert.Equal(value, _store.Read(key));
      }

      [Fact]
      public void Write_AddValueWithEqualToSignDelimiter_ShouldWriteAndReadCorrectly()
      {
         if (!_store.CanWrite) return;

         _store.Write("key9", "value=9");

         Assert.Equal("value=9", _store.Read("key9"));
      }

      [Fact]
      public void Read_NonExistingKey_ReturnsNull()
      {
         if (!_store.CanRead) return;

         string value = _store.Read(Guid.NewGuid().ToString());

         Assert.Null(value);
      }

      [Fact]
      public void Read_Null_Null()
      {
         if (!_store.CanRead) return;

         string value = _store.Read(null);

         Assert.Null(value);
      }

      public void Dispose()
      {
         _store.Dispose();
      }
   }
}
