using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Config.Net.Tests
{
   public partial class VirtualStoreTest
   {
      [Theory]
      [InlineData("testkey", "testvalue")]
      [InlineData("testkey1", "34567")]
      [InlineData("testkey2", "HOMER,BART,LISA,MARGE,MAGGI")]
      [InlineData("testkey3", null)]
      public void Write_WritesKeyValue_ReadsBackCorrectly(string key, string value)
      {
         if (!store.CanWrite) return;

         store.Write(key, value);

         Assert.Equal(value, store.Read(key));
      }

      [Fact]
      public void Write_ReplacesValue_ReadsBackCorrectly()
      {
         if (!store.CanWrite) return;

         const string key = "key7";
         const string value = "changedvalue7";

         store.Write(key, value);

         Assert.Equal(value, store.Read(key));
      }

      [Fact]
      public void Write_AddValueWithEqualToSignDelimiter_ShouldWriteAndReadCorrectly()
      {
         if (!store.CanWrite) return;

         store.Write("key9", "value=9");

         Assert.Equal("value=9", store.Read("key9"));
      }

      [Fact]
      public void Read_NonExistingKey_ReturnsNull()
      {
         if (!store.CanRead) return;

         string value = store.Read(Guid.NewGuid().ToString());

         Assert.Null(value);
      }

      [Fact]
      public void Read_Null_Null()
      {
         if (!store.CanRead) return;

         string value = store.Read(null);

         Assert.Null(value);
      }
   }
}
