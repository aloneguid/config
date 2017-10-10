using Config.Net.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Config.Net.Stores.Impl.CommandLine;

namespace Config.Net.Tests
{
   public class CommandLineConfigStoreTest
   {
      [Fact]
      public void NamedParameters_MixedNaming_ParsesValues()
      {
         var store = new CommandLineConfigStore(new[] { "-v:123", "--version:123", "-x5", "nothing" });

         string v = store.Read("v");
         Assert.Equal("123", v);
      }

   }
}