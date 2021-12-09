using System.IO;
using Config.Net.Json.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Config.Net.Stores;
using Xunit;

namespace Config.Net.Tests
{
   public class MultipleConfigurationFilesTest : AbstractTestFixture
   {
      private IRootElement ConfigInterface { get; }

      private const string configBasic = @"{
'KeyA': 'basic',
'KeyB': 'basic',
    'Creds': [
      {
         'Username': 'user',
         'Password': 'debug'
   }
]
}";

      private const string configOverride = @"{
'KeyB': 'override',
}";

      public interface IRootElement
      {
         string KeyA { get; }
         string KeyB { get; }
         IEnumerable<IArrayElement> Creds { get; }
      }

      public interface IArrayElement
      {
         string Username { get; }

         string Password { get; }
      }

      public MultipleConfigurationFilesTest()
      {
         var configurationBuilder = new ConfigurationBuilder<IRootElement>();
         configurationBuilder
            .UseJsonString(configOverride)
            .UseJsonString(configBasic);
         ConfigInterface = configurationBuilder.Build();
      }

      [Fact]
      public void Read_Correct_Key()
      {
         Assert.Equal("basic", ConfigInterface.KeyA);
         Assert.Equal("override", ConfigInterface.KeyB);
      }

      [Fact]
      public void Read_Creds_IsNotEmpty()
      {
         Assert.NotEmpty(ConfigInterface.Creds);
      }

      [Fact]
      public void Read_Correct_Creds()
      {
         Assert.Equal("user", ConfigInterface.Creds.ToArray().FirstOrDefault()?.Username);
         Assert.Equal("debug", ConfigInterface.Creds.FirstOrDefault()?.Password);
      }
   }
}