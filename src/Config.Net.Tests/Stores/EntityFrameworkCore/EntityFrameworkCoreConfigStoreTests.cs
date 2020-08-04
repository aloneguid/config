using System;
using System.IO;
using Config.Net.EntityFrameworkCore;
using Config.Net.EntityFrameworkCore.Stores;
using Xunit;

namespace Config.Net.Tests.Stores.EntityFrameworkCore
{
   public class EntityFrameworkCoreConfigStoreTests : AbstractTestFixture, IDisposable
   {
      private readonly string _currentFile;
      private readonly EntityFrameworkCoreConfigStore<TestContext, Setting> _store;

      public EntityFrameworkCoreConfigStoreTests()
      {
         string sample = Path.Combine(BuildDir.FullName, "TestData", "sample.db");
         _currentFile = Path.Combine(BuildDir.FullName, "TestData", "current.db");
         File.Copy(sample, _currentFile, true);
         _store = new EntityFrameworkCoreConfigStore<TestContext, Setting>(
            new TestContext(_currentFile),
            key => setting => setting.Key == key,
            setting => setting.Value,
            (setting, value) => setting.Value = value,
            (setting, key) => setting.Key = key);
         _store.Write("somekey", "bla");
      }

      [Fact]
      public void Read_existing()
      {
         string value = _store.Read("some_key");
         Assert.NotNull(value);
      }

      [Fact]
      public void Read_unknown()
      {
         string value = _store.Read("unknown_key");
         Assert.Null(value);
      }

      [Fact]
      public void Write_create_new()
      {
          _store.Write("new_key", "123");
         Assert.Equal("123", _store.Read("new_key"));
      }

      [Fact]
      public void Write_edit_existing()
      {
         _store.Write("some_key", "123");
         Assert.Equal("123", _store.Read("some_key"));
      }

      public void Dispose()
      {
         _store.Dispose();
         File.Delete(_currentFile);
      }
   }
}