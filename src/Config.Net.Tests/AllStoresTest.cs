using System;
using System.IO;
#if NETFULL
using Config.Net.Azure;
#endif
using Config.Net.Stores;
using Xunit;

namespace Config.Net.Tests
{
   public class IniFileTest : AllStoresTest
   {
      protected override IConfigStore CreateStore()
      {
         string dir = BuildDir.FullName;
         string src = Path.Combine(dir, "TestData", "example.ini");
         string testFile = Path.Combine(dir, "test.ini");
         File.Copy(src, testFile, true);
         return new IniFileConfigStore(testFile);
      }
   }

#if NETFULL
   public class AzureTablesTest : AllStoresTest
   {
      protected override IConfigStore CreateStore()
      {
         return new AzureTableConfigStore(
            _settings.AzureStorageName,
            _settings.AzureStorageKey,
            "configurationtest", "confignettests");
      }
   }
#endif

   public class InMemoryTest : AllStoresTest
   {
      protected override IConfigStore CreateStore()
      {
         return new InMemoryConfigStore();
      }
   }

#if NETFULL
   public class AppConfigTest : AllStoresTest
   {
      protected override IConfigStore CreateStore()
      {
         return new AppConfigStore();
      }
   }
#endif

   public class EnvironmentVariablesTest : AllStoresTest
   {
      protected override IConfigStore CreateStore()
      {
         return new EnvironmentVariablesStore();
      }
   }

   public class CommandLineStoreTest : AllStoresTest
   {
      protected override IConfigStore CreateStore()
      {
         return new CommandLineConfigStore(null);
      }
   }


#if NETFULL
   public class AzureKeyVaultTest : AllStoresTest
   {
      protected override IConfigStore CreateStore()
      {
         return new AzureKeyVaultConfigStore(
                  _settings.AzureKeyVaultUri,
                  _settings.AzureKeyVaultClientId,
                  _settings.AzureKeyVaultSecret);
      }
   }
#endif

   /// <summary>
   /// Tests all stores for consistent behavior
   /// </summary>
   public abstract class AllStoresTest : AbstractTestFixture, IDisposable
   {
      private IConfigStore _store;
      //private readonly string _storeName;
      private string _testFile;
      protected TestSettings _settings = new TestSettings();

      protected abstract IConfigStore CreateStore();
      public AllStoresTest()
      {
         _store = CreateStore();
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
