using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Config.Net.Tests
{
   public class ConfigurationTest
   {
      //[Fact]
      public void Call_method_with_section_names()
      {
         ISettings settings = new ConfigurationBuilder<ISettings>()
            .Build();

         int n = settings.GetNumber("s1", "s2");

         //settings.SetNumber(5);
      }

      [Fact]
      public void Derived_interfaces_can_still_access_base_interfaces()
      {
         IDerivedSettings settings = new ConfigurationBuilder<IDerivedSettings>()
            .Build();

         Assert.Equal(7, settings.AnotherNumber);
         Assert.Equal(3, settings.Number);
      }
   }

   public interface ISettings
   {
      [Option(DefaultValue = 3)]
      int Number { get; }

      int GetNumber(

         [Option(Alias = "Azure")]
         string section1Name,
         
         string section2Name);

      void SetNumber(int value);
   }

   public interface IDerivedSettings : ISettings
   {
      [Option(DefaultValue = 7)]
      int AnotherNumber { get; }
   }
}
