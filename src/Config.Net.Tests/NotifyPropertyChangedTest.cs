using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Config.Net.Stores;
using Xunit;

namespace Config.Net.Tests
{
   public class NotifyPropertyChangedTest
   {
      private INPC _interface;
      private DictionaryConfigStore _store;

      public NotifyPropertyChangedTest()
      {
         _store = new DictionaryConfigStore();

         _interface = new ConfigurationBuilder<INPC>()
            .UseConfigStore(_store)
            .Build();
      }

      [Fact]
      public void Change_property_calls_notify()
      {
         bool isSet = false;
         string nameSet = null;

         _interface.PropertyChanged += (sender, e) =>
         {
            isSet = true;
            nameSet = e.PropertyName;
         };

         _interface.Name = "test";
         Assert.True(isSet);
         Assert.Equal("Name", nameSet);
      }

      [Fact]
      public void Change_aliased_property_calls_notify()
      {
         bool isSet = false;
         string nameSet = null;

         _interface.PropertyChanged += (sender, e) =>
         {
            isSet = true;
            nameSet = e.PropertyName;
         };

         _interface.AliasedName = "test";
         Assert.True(isSet);
         Assert.Equal("Name1", nameSet);
      }
   }

   public interface INPC : INotifyPropertyChanged
   {
      string Name { get; set; }

      [Option(Alias = "Name1")]
      string AliasedName { get; set; }
   }

}
