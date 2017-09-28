using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Config.Net.Tests
{
   public class WorkflowTest
   {
      [Fact]
      public void Smoke()
      {
         IServerSettings settings = 
            new ConfigurationBuilder<IServerSettings>()
            .UseEnvironmentVariables()
            .UseCommandLineArgs()
            .Build();

         string address = settings.Address;
         string s = settings.Key;
      }

   }

   public interface IServerSettings
   {
      string Address { get; set; }

      [Option(Name = "key", DefaultValue = "nokey")]
      string Key { get; set; }
   }
}
