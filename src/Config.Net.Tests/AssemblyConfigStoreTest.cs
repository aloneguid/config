using System;
using System.IO;
using System.Reflection;
using Config.Net.Stores;
using NUnit.Framework;

namespace Config.Net.Tests
{
   [TestFixture]
   public class AssemblyConfigStoreTest
   {
      private string _shadowCopyFile;

      [SetUp]
      public void EnsureConfigFile()
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

      [Test]
      public void ReadSettingTest()
      {
         IConfigStore store = new AssemblyConfigStore(Assembly.GetExecutingAssembly());
         Assert.IsNull(store.Read("AnyKeyYouWant"));
      }

      [TearDown]
      public void DeleteShadowCopyFile()
      {
         if (!string.IsNullOrEmpty(_shadowCopyFile))
            File.Delete(_shadowCopyFile);
      }
   }
}