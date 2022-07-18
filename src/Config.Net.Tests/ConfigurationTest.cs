using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace Config.Net.Tests
{
   public class ConfigurationTest
   {

      [Fact]
      public void Derived_interfaces_can_still_access_base_interfaces()
      {
         IDerivedSettings settings = new ConfigurationBuilder<IDerivedSettings>()
            .Build();

         Assert.Equal(7, settings.AnotherNumber);
         Assert.Equal(3, settings.Number);
      }

      [Fact]

      public void CreateJsonFileAndRead()
      {
         string filename = "testFile.json";
         if (File.Exists(filename))
         {
            File.Delete(filename);
         }

         IDerivedSettings settings = new ConfigurationBuilder<IDerivedSettings>()
            .UseJsonFile(filename)
            .Build();

         Assert.Equal(7, settings.AnotherNumber);
         Assert.Equal(3, settings.Number);

         settings.NumberThird = 9;
         settings.NumberFourth = 1;

         IDerivedSettings settings2 = new ConfigurationBuilder<IDerivedSettings>()
            .UseJsonFile(filename)
            .Build();

         Assert.Equal(9, settings2.NumberThird);
         Assert.Equal(1, settings2.NumberFourth);

      }


      [Fact]
      public void Invalid_basic_type_prevents_builder()
      {
         Assert.Throws<NotSupportedException>(() => new ConfigurationBuilder<IInvalidBasicType>().Build());
      }

      [Fact]
      public void Take_value_for_second_from_first()
      {
         //construct the builder first
         var builder = new ConfigurationBuilder<ITwoSettings>();

         //add a sample store that contains setings for future stores
         builder.UseInMemoryDictionary(new Dictionary<string, string> { ["OneForSecond"] = "first" });

         //get the value before you finish building all the settings container
         string second = builder.Build().OneForSecond;

         Assert.Equal("first", second);

         // add another store, by using a setting from the previous one
         builder.UseInMemoryDictionary(new Dictionary<string, string> { ["Second"] = second });

         // assert
         ITwoSettings settings = builder.Build();
         Assert.Equal("first", settings.OneForSecond);
         Assert.Equal("first", settings.Second);

      }
   }

   public interface IInvalidBasicType
   {
      Type NotSupported { get; }
   }

   public interface ISettings
   {
      [Option(DefaultValue = 3)]
      int Number { get; }

      int GetNumber(

         string section1Name,

         string section2Name);

      void SetNumber(int value);
   }

   public interface IDerivedSettings : ISettings
   {
      [Option(DefaultValue = 7)]
      int AnotherNumber { get; }
      int NumberThird { get; set; }
      int NumberFourth { get; set; }
   }

   public interface ITwoSettings
   {
      string OneForSecond { get; }

      string Second { get; }
   }
}
