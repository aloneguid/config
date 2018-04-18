using System;
using System.IO;
using System.Threading;
using Config.Net.Stores;
using Xunit;

namespace Config.Net.Tests.Stores
{
   public class IniFileConfigStoreTest : AbstractTestFixture, IDisposable
   {
      private string _testFilePath;
      private IniFileConfigStore _store;

      public IniFileConfigStoreTest()
      {
         //get back clean file
         string src = Path.Combine(BuildDir.FullName, "TestData", "example.ini");
         _testFilePath = Path.Combine(TestDir.FullName, "example.ini");
         File.Copy(src, _testFilePath, true);

         //create the store
         _store = new IniFileConfigStore(_testFilePath, true, true);
      }

      public void Dispose()
      {
         _store.Dispose();
      }

      [Fact]
      public void Read_Sectionless_CorrectValue()
      {
         Assert.Equal("svalue0", _store.Read("skey0"));
      }

      [Fact]
      public void Read_SectionOne_CorrectValue()
      {
         Assert.Equal("s1value0", _store.Read("SectionOne.key0"));
      }

      [Fact]
      public void Read_KeyWithComment_CorrectValue()
      {
         Assert.Equal("s1value1", _store.Read("SectionOne.key1"));
      }

      [Fact]
      public void Read_FileDoesNotExist_DoesNotFail()
      {
         var store = new IniFileConfigStore($"c:\\{Guid.NewGuid()}.ini", true, true);
      }

      [Fact]
      public void ValuesChange_RewriteFileWithNewValues_DoesNotReadNewValuesBecauseTheyreCached()
      {
         File.WriteAllText(_testFilePath, "keyV=valueV");

         //Assert.Equal("valueV", _store.Read("keyV")); //this may fail immediately because it takes time for FSW to pick up the changes

         Thread.Sleep(TimeSpan.FromSeconds(1));

         Assert.NotEqual("valueV", _store.Read("keyV"));
      }

      [Fact]
      public void Write_NewFileWithNewValues_WritesCorrectText()
      {
         string fullPath = Path.Combine(TestDir.FullName, Guid.NewGuid() + ".ini");
         var ini = new IniFileConfigStore(fullPath, true, true);

         ini.Write("key0", "value0");
         ini.Write("S1.key0", "s1value0");
         ini.Write("S2.key0", "s2value0");

         string resultText = File.ReadAllText(fullPath);

         Assert.Equal(
            @"key0=value0

[S1]
key0=s1value0

[S2]
key0=s2value0
", resultText, false, true);
      }
   }
}