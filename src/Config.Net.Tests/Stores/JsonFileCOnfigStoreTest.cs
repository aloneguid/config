using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Config.Net.Stores;
using Xunit;

namespace Config.Net.Tests.Stores
{
   public class JsonFileCOnfigStoreTest : AbstractTestFixture, IDisposable
   {
      private string _path;
      private JsonFileConfigStore _store;

      public JsonFileCOnfigStoreTest()
      {
         _path = Path.Combine(BuildDir.FullName, "TestData", "appSettings.json");
         _store = new JsonFileConfigStore(_path);
      }

      [Fact]
      public void Read_inline_property()
      {
         string key = _store.Read("ApplicationInsights.InstrumentationKey");

         Assert.NotNull(key);
      }

      [Fact]
      public void Write_inline_property_reads_back()
      {
         string key = "ApplicationInsights.InstrumentationKey";

         _store.Write(key, "123");

         Assert.Equal("123", _store.Read(key));
      }

      [Fact]
      public void Write_new_value_hierarchically()
      {
         string key = "One.Two.Three";

         _store.Write(key, "111");

         Assert.Equal("111", _store.Read(key));
      }

      public void Dispose()
      {
         _store.Dispose();
      }
   }
}
