using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Config.Net.Json.Stores;
using Config.Net.Stores;
using Config.Net.Stores.Impl.CommandLine;
using Config.Net.Yaml.Stores;

namespace Config.Net.Tests
{
   public abstract partial class VirtualStoreTest : AbstractTestFixture, IDisposable
   {
      protected IConfigStore store;

      public VirtualStoreTest()
      {
         store = CreateStore();
      }

      protected virtual IConfigStore CreateStore()
      {
         throw new NotImplementedException();
      }

      public void Dispose()
      {
         store.Dispose();
      }
   }

   #region [ Set Up Variations ]

   public class IniFileStoreTest : VirtualStoreTest
   {
      protected override IConfigStore CreateStore()
      {
         string dir = BuildDir.FullName;
         string src = Path.Combine(dir, "TestData", "sample.ini");
         string testFile = Path.Combine(dir, "sample.ini");
         File.Copy(src, testFile, true);
         return new IniFileConfigStore(testFile, true, false);
      }
   }

   public class IniFileContentStoreTest : VirtualStoreTest
   {
      protected override IConfigStore CreateStore()
      {
         string dir = BuildDir.FullName;
         string src = Path.Combine(dir, "TestData", "sample.ini");
         string content = File.ReadAllText(src);

         return new IniFileConfigStore(content, false, true);
      }
   }

   public class YamlConfigStoreTest : VirtualStoreTest
   {
      protected override IConfigStore CreateStore()
      {
         string testFile = Path.Combine(BuildDir.FullName, "TestData", "sample.yml");
         return new YamlFileConfigStore(testFile);
      }
   }

   public class InMemoryTest : VirtualStoreTest
   {
      protected override IConfigStore CreateStore()
      {
         return new InMemoryConfigStore();
      }
   }

#if NETFULL
   public class AppConfigTest : VirtualStoreTest
   {
      protected override IConfigStore CreateStore()
      {
         return new AppConfigStore();
      }
   }
#endif

   public class EnvironmentVariablesTest : VirtualStoreTest
   {
      protected override IConfigStore CreateStore()
      {
         return new EnvironmentVariablesStore();
      }
   }

   public class CommandLineStoreTest : VirtualStoreTest
   {
      protected override IConfigStore CreateStore()
      {
         return new CommandLineConfigStore(null);
      }
   }

   public class JsonFileConfigStoreTest : VirtualStoreTest
   {
      protected override IConfigStore CreateStore()
      {
         string testFile = Path.Combine(BuildDir.FullName, "test.json");
         return new JsonFileConfigStore(testFile, true);
      }
   }

   public class JsonStringConfigStoreTest : VirtualStoreTest
   {
      protected override IConfigStore CreateStore()
      {
         string testFile = Path.Combine(BuildDir.FullName, "TestData", "sample.json");
         string json = File.ReadAllText(testFile);
         return new JsonFileConfigStore(json, false);
      }
   }

   #endregion
}
