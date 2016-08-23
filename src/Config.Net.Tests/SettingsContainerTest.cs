using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Config.Net;
using NUnit.Framework;

namespace Config.Net.Tests
{
   [TestFixture]
   public class SettingsContainerTest
   {
      class MyContainer : SettingsContainer
      {
         public MyContainer() : base("MyApp")
         {
         }

         public int Timeout { get; set; }

         [Option(Name = "StrongestSpan")]
         public TimeSpan StrongSpan { get; set; }

         protected override void OnConfigure(IConfigConfiguration configuration)
         {
            configuration.UseEnvironmentVariables();
         }
      }

      [Test]
      public void Smoke()
      {
         var c = new MyContainer();

         int timeout = c.Timeout;
      }
   }
}
