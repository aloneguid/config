using Config.Net.Stores;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Config.Net.Tests
{
   public class ConfigurableMethodsTest
   {
      private ICallableConfig _config;
      private InMemoryConfigStore _store;

      public ConfigurableMethodsTest()
      {
         _store = new InMemoryConfigStore();

         _config = new ConfigurationBuilder<ICallableConfig>()
            .UseConfigStore(_store)
            .Build();
      }

      [Fact]
      public void Call_simple_get_method()
      {
         _store.Write("First.section.key", "mv");

         string value = _config.GetFirst("section", "key");

         Assert.Equal("mv", value);
      }

      [Fact]
      public void Call_method_with_alias()
      {
         _store.Write("A.section", "mv");

         string value = _config.GetSecond("section");

         Assert.Equal("mv", value);
      }

      [Fact]
      public void Call_get_default_value()
      {
         string value = _config.GetThird("whatever");

         Assert.Equal("n/a", value);
      }

      [Fact]
      public void Call_second_level_method()
      {
         _store.Write("Nested.Nested.section", "nested");

         string value = _config.Nested.GetNested("section");

         Assert.Equal("nested", value);
      }

      [Fact]
      public void Set_simple_method()
      {
         _config.SetFirst("one");

         Assert.Equal("one", _store.Read("First"));
      }
   }

   public interface ICallableConfig
   {
      string GetFirst(string sectionName, string key);

      [Option(Alias = "A")]
      string GetSecond(string sectionName);

      [Option(DefaultValue = "n/a")]
      string GetThird(string name);

      void SetFirst(string name);

      INestedCallableConfig Nested { get; }
   }

   public interface INestedCallableConfig
   {
      string GetNested(string sectionName);
   }

}
