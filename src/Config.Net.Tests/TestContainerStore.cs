using System;
using System.Collections.Generic;
using System.IO;
using Config.Net.Stores;
using Xunit;

namespace Config.Net.Tests
{
   public class StoreOrderSettings : SettingsContainer
   {
      private readonly bool _reverse;
      public Option<string> a { get; } = new Option<string>("a", "DEFAULT A");
      public Option<string> b { get; } = new Option<string>("b", "DEFAULT B");
      public Option<string> c { get; } = new Option<string>("c", "DEFAULT C");
      public Option<string> x { get; } = new Option<string>("x", "DEFAULT X");

      public StoreOrderSettings(bool reverse = false)
      {
         _reverse = reverse;
      }

      protected override void OnConfigure(IConfigConfiguration configuration)
      {
         configuration.CacheTimeout = TimeSpan.FromMinutes(1);
         if (_reverse)
         {
            configuration.UseJsonFile(@".\low.json");
            configuration.UseJsonFile(@".\medium.json");
            configuration.UseJsonFile(@".\high.json");
         }
         else
         {
            configuration.UseJsonFile(@".\high.json");
            configuration.UseJsonFile(@".\medium.json");
            configuration.UseJsonFile(@".\low.json");
         }
      }
   }

   public class StoreOrderTestClass
   {
      public StoreOrderTestClass()
      {
         File.WriteAllText(@".\low.json",   "{ \"a\":\"LOW-A\", \"b\":\"LOW-B\", \"c\":\"LOW-C\"}");
         File.WriteAllText(@".\medium.json","{ \"a\":\"MED-A\", \"b\":\"MED-B\" }");
         File.WriteAllText(@".\high.json",  "{ \"a\":\"HIGH-A\" }");
      }

      [Fact]
      public void Test_Store_Priority_Order()
      {
         var container = new StoreOrderSettings();
         Assert.Equal("HIGH-A", container.a);
         Assert.Equal("MED-B", container.b);
         Assert.Equal("LOW-C", container.c);
         Assert.Equal("DEFAULT X", container.x);
      }
      
      [Fact]
      public void Test_Store_Priority_Order_Reverse()
      {
         var container = new StoreOrderSettings(true);
         Assert.Equal("LOW-C", container.c);
         Assert.Equal("LOW-B", container.b);
         Assert.Equal("LOW-A", container.a);
         Assert.Equal("DEFAULT X", container.x);
      }

   }
}
