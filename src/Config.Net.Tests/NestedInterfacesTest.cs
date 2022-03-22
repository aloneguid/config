using System;
using System.Collections.Generic;
using System.Text;
using Config.Net.Stores;
using Xunit;

namespace Config.Net.Tests
{
   public class NestedInterfacesTest
   {
      private IMaster _config;
      private DictionaryConfigStore _store;

      public NestedInterfacesTest()
      {
         _store = new DictionaryConfigStore();

         _config = new ConfigurationBuilder<IMaster>()
            .UseConfigStore(_store)
            .Build();
      }

      [Fact]
      public void Read_both_usernames()
      {
         _store.Write("Admin.Username", "u1");
         _store.Write("Normal.Username", "u2");

         string u1 = _config.Admin?.Username;
         string u2 = _config.Normal?.Username;

         Assert.Equal("u1", u1);
         Assert.Equal("u2", u2);
      }

      [Fact]
      public void Read_third_level_interface()
      {
         _store.Write("Admin.Domain.Name", "dn");

         string domain = _config.Admin.Domain.Name;

         Assert.Equal("dn", domain);
      }

      [Fact]
      public void Read_by_alias()
      {
         _store.Write("Admin.Domain.addr", "dn");

         string address = _config.Admin.Domain.Address;

         Assert.Equal("dn", address);

      }
   }

   public interface IMaster
   {
      string Name { get; }

      ICreds Admin { get; }

      ICreds Normal { get; }
   }

   public interface ICreds
   {
      string Username { get; }

      string Password { get; }

      IDomain Domain { get; }
   }

   public interface IDomain
   {
      string Name { get; }

      [Option(Alias = "addr")]
      string Address { get; }
   }

}
