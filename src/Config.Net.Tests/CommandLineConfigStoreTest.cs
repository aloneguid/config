using Config.Net.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Config.Net.Tests
{
   public class CommandLineConfigStoreTest
   {
      [Fact]
      public void NamedParameters_MixedNaming_ParsesValues()
      {
         var store = new CommandLineConfigStore(new[] { "-v:123", "--version:123", "-x5", "nothing" }, null);

         string v = store.Read("v");
         Assert.Equal("123", v);
      }

      [Fact]
      public void PositionalParameters_returned_as_value()
      {
         var store = new CommandLineConfigStore(new[] { "create", "name=thename" },
            new Dictionary<int, Option>
            {
               {0, new Option<string>("positional", null) }
            });


         Assert.Equal("create", store.Read("positional"));
         Assert.Equal("thename", store.Read("name"));

      }
   }
}