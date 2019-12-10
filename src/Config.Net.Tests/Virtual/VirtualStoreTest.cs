using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Config.Net;
using Config.Net.Azure.KeyVault;
using Config.Net.Json.Stores;
using Config.Net.Stores;
using Config.Net.Stores.Impl.CommandLine;
using Config.Net.Yaml.Stores;

namespace Config.Net.Tests.Virtual
{
   public abstract partial class VirtualStoreTest : AbstractTestFixture, IDisposable
   {
      protected IConfigStore store;
      protected ITestCredential creds;
      protected bool isPrepopulated = true;

      public VirtualStoreTest()
      {
         creds = new ConfigurationBuilder<ITestCredential>()
            .UseIniFile("c:\\tmp\\integration-tests.ini")
            .UseEnvironmentVariables()
            .Build();

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
         var result = new DictionaryConfigStore();
         result.Write("Numbers", "1 2 3");
         result.Write("Creds[0].Username", "user1");
         result.Write("Creds[0].Password", "pass1");
         result.Write("Creds[1].Username", "user2");
         result.Write("Creds[1].Password", "pass2");
         result.Write("Creds.$l", "2");
         return result;
      }
   }

   //this probably requires an external process project
   /*public class AppConfigTest : VirtualStoreTest
   {
      protected override IConfigStore CreateStore()
      {
         return new AppConfigStore();
      }
   }*/

   public class EnvironmentVariablesTest : VirtualStoreTest
   {
      protected override IConfigStore CreateStore()
      {
         Environment.SetEnvironmentVariable("Numbers", "1 2 3");
         Environment.SetEnvironmentVariable("Creds[0].Username", "user1");
         Environment.SetEnvironmentVariable("Creds[0].Password", "pass1");
         Environment.SetEnvironmentVariable("Creds[1].Username", "user2");
         Environment.SetEnvironmentVariable("Creds[1].Password", "pass2");
         Environment.SetEnvironmentVariable("Creds.$l", "2");

         return new EnvironmentVariablesStore();
      }
   }

   public class CommandLineStoreTest : VirtualStoreTest
   {
      protected override IConfigStore CreateStore()
      {
         return new CommandLineConfigStore(new[]
         {
            "Numbers=1 2 3",
            "Creds[0].Username=user1",
            "Creds[0].Password=pass1",
            "Creds[1].Username=user2",
            "Creds[1].Password=pass2",
            "Creds.$l=2"
         });
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

   public class AzureKeyVaultConfigStoreTest : VirtualStoreTest
   {
      protected override IConfigStore CreateStore()
      {
         isPrepopulated = false;

         return AzureKeyVaultConfigStore.CreateWithPrincipal(creds.AzureKeyVaultUri, creds.AzureKeyVaultCredentials.UserName, creds.AzureKeyVaultCredentials.Password);
      }
   }

   public class AzureDevOpsVariableSetConfigStoreTest : VirtualStoreTest
   {
      protected override IConfigStore CreateStore()
      {
         return new AzureDevOpsVariableSetConfigStore(creds.AzDevOpsOrg, creds.AzDevOpsProject, creds.AzDevOpsPat, creds.AzDeOpsVarId);
      }
   }

   #endregion
}
