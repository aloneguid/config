using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Config.Net.Json.Stores;
using Config.Net.Stores;
using Config.Net.Stores.Impl.CommandLine;
using Config.Net.Yaml.Stores;

namespace Config.Net.Tests.Virtual
{
   public abstract partial class VirtualStoreTest : AbstractTestFixture, IDisposable
   {
      protected IConfigStore store;

      public VirtualStoreTest()
      {
         store = CreateStore();
      }

      protected string GetSamplePath(string ext)
      {
         string dir = BuildDir.FullName;
         string src = Path.Combine(dir, "TestData", "sample." + ext);
         string testFile = Path.Combine(dir, src);
         src = Path.GetFullPath(testFile);
         string dest = Path.Combine(TestDir.FullName, "sample." + ext);

         File.Copy(src, dest, true);

         return dest;
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
         string testFile = GetSamplePath("ini");
         return new IniFileConfigStore(testFile, true, false);
      }
   }

   public class IniFileContentStoreTest : VirtualStoreTest
   {
      protected override IConfigStore CreateStore()
      {
         string src = GetSamplePath("ini");
         string content = File.ReadAllText(src);

         return new IniFileConfigStore(content, false, true);
      }
   }

   public class YamlConfigStoreTest : VirtualStoreTest
   {
      protected override IConfigStore CreateStore()
      {
         string testFile = GetSamplePath("yml");
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
         string testFile = GetSamplePath("json");
         return new JsonFileConfigStore(testFile, true);
      }
   }

   public class JsonStringConfigStoreTest : VirtualStoreTest
   {
      protected override IConfigStore CreateStore()
      {
         string testFile = GetSamplePath("json");
         string json = File.ReadAllText(testFile);
         return new JsonFileConfigStore(json, false);
      }
   }

   #endregion
}
