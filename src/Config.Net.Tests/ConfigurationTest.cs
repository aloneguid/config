using System;
using System.Collections.Generic;
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
      public void Invalid_basic_type_prevents_builder()
      {
         Assert.Throws<NotSupportedException>(() => new ConfigurationBuilder<IInvalidBasicType>().Build());
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
   }
}
