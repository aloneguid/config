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
      public readonly Option<string> a = new Option<string>("a", "DEFAULT A");
      public readonly Option<string> b = new Option<string>("b", "DEFAULT B");
      public readonly Option<string> c = new Option<string>("c", "DEFAULT C");
      public readonly Option<string> x = new Option<string>("x", "DEFAULT X");

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
         Assert.Equal(container.a , "HIGH-A");
         Assert.Equal(container.b , "MED-B");
         Assert.Equal(container.c , "LOW-C");
         Assert.Equal(container.x , "DEFAULT X");
      }
      
      [Fact]
      public void Test_Store_Priority_Order_Reverse()
      {
         var container = new StoreOrderSettings(true);
         Assert.Equal(container.c , "LOW-C");
         Assert.Equal(container.b , "LOW-B");
         Assert.Equal(container.a , "LOW-A");
         Assert.Equal(container.x , "DEFAULT X");
      }

   }
}
