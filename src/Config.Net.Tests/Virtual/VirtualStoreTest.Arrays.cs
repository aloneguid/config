using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace Config.Net.Tests.Virtual
{
   public partial class VirtualStoreTest
   {
      [Fact]
      public void Read_collection_of_interfaces()
      {
         IInterfaceArrays config = new ConfigurationBuilder<IInterfaceArrays>()
             .UseConfigStore(store)
             .Build();

         IEnumerable<IArrayElement> r = config.Creds;

         List<IArrayElement> rl = r.ToList();

         Assert.Equal(2, rl.Count);

         IArrayElement el0 = rl[0];
         IArrayElement el1 = rl[1];

         Assert.Equal("user1", el0.Username);
         Assert.Equal("pass1", el0.Password);
         Assert.Equal("user2", el1.Username);
         Assert.Equal("pass2", el1.Password);
      }

      [Fact]
      public void Read_collection_of_simple_values()
      {
         ISimpleArrays config = new ConfigurationBuilder<ISimpleArrays>()
            .UseConfigStore(store)
            .Build();

         List<int> numbers = config.Numbers.ToList();

         Assert.Equal(new int[] { 1, 2, 3 }, numbers);
      }

      [Fact]
      public void Collections_cannot_have_a_setter()
      {
         var builder = new ConfigurationBuilder<ISetterArrays>();

         Assert.Throws<NotSupportedException>(() => builder.Build());
      }
   }

   public interface IInterfaceArrays
   {
      IEnumerable<IArrayElement> Creds { get; }
   }

   public interface ISimpleArrays
   {
      IEnumerable<int> Numbers { get; }
   }

   public interface IArrayElement
   {
      string Username { get; }

      string Password { get; }
   }

   public interface ISetterArrays
   {
      IEnumerable<int> WithSettter { get; set; }
   }
}
