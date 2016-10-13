using System;
using System.IO;
using System.Reflection;
using Config.Net.Stores;
using Xunit;

namespace Config.Net.Tests
{
   public class AssemblyConfigStoreTest
   {
      private string _shadowCopyFile;

      public AssemblyConfigStoreTest()
      {
         Assembly asm = Assembly.GetExecutingAssembly();

         if (!string.IsNullOrEmpty(asm.Location))
         {
            string config = asm.Location + ".config";

            if (!File.Exists(config))
            {
               string source = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath + ".config";
               File.Copy(source, config, true);
               _shadowCopyFile = config;
            }
         }
      }

      [Fact]
      public void ReadSettingTest()
      {
         IConfigStore store = new AssemblyConfigStore(Assembly.GetExecutingAssembly());
         Assert.Null(store.Read("AnyKeyYouWant"));
      }

      public void Dispose()
      {
         if (!string.IsNullOrEmpty(_shadowCopyFile))
            File.Delete(_shadowCopyFile);
      }
   }
}