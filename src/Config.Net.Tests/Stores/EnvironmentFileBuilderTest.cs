using System;
using System.IO;
using Config.Net.TypeParsers;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Config.Net.Tests.Stores
{
   public class EnvironmentFileBuilderTest : AbstractTestFixture, IDisposable
   {
      private readonly string _path;

      public EnvironmentFileBuilderTest()
      {
         _path = Path.Combine(BuildDir.FullName, "TestData", "sample.json");
      }

      public void Dispose()
      {
         Environment.SetEnvironmentVariable(EnvironmentFileBuilder.EnvironmentKey, null);
      }

      [Fact]
      public void Build_NoEnvVarNoFile_EmptyConfigStore()
      {
         IConfigStore configStore = new EnvironmentFileBuilder().Build(_path + "x");

         Assert.Null(configStore.Read("ApplicationInsights.InstrumentationKey"));
      }

      [Fact]
      public void Build_NoEnvVarFileExist_ProductionConfigStore()
      {
         IConfigStore configStore = new EnvironmentFileBuilder().Build(_path);

         Assert.False(bool.Parse(configStore.Read("Logging.IncludeScopes")));

         Assert.Equal("Debug", configStore.Read("Logging.LogLevel.Default"));
         Assert.Equal("Information", configStore.Read("Logging.LogLevel.System"));
         Assert.Equal("Information", configStore.Read("Logging.LogLevel.Microsoft"));

         Assert.Equal("c75aaedd-7e93-4f67-b7b7-526f7924ccaa", configStore.Read("ApplicationInsights.InstrumentationKey"));

         Assert.Equal("1", configStore.Read("Numbers[0]"));
         Assert.Equal("2", configStore.Read("Numbers[1]"));
         Assert.Equal("3", configStore.Read("Numbers[2]"));

         Assert.Equal("user1", configStore.Read("Creds[0].Username"));
         Assert.Equal("pass1", configStore.Read("Creds[0].Password"));
         Assert.Equal("user2", configStore.Read("Creds[1].Username"));
         Assert.Equal("pass2", configStore.Read("Creds[1].Password")); ;
      }

      [Fact]
      public void Build_DebugEnvVarFileExist_DebugOverrideConfigStore()
      {
         Environment.SetEnvironmentVariable(EnvironmentFileBuilder.EnvironmentKey, "Debug");
         IConfigStore configStore = new EnvironmentFileBuilder().Build(_path);

         Assert.True(bool.Parse(configStore.Read("Logging.IncludeScopes")));

         Assert.Equal("Debug", configStore.Read("Logging.LogLevel.Default"));
         Assert.Equal("Information", configStore.Read("Logging.LogLevel.System"));
         Assert.Equal("Debug", configStore.Read("Logging.LogLevel.Microsoft"));

         Assert.Equal("debug-key", configStore.Read("ApplicationInsights.InstrumentationKey"));

         Assert.Equal("1", configStore.Read("Numbers[0]"));
         Assert.Equal("0", configStore.Read("Numbers[1]"));
         Assert.Equal("3", configStore.Read("Numbers[2]"));

         Assert.Equal("user1", configStore.Read("Creds[0].Username"));
         Assert.Equal("pass1", configStore.Read("Creds[0].Password"));
         Assert.Equal("user2", configStore.Read("Creds[1].Username"));
         Assert.Equal("debug", configStore.Read("Creds[1].Password"));
      }
   }
}