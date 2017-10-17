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
   }

   public interface ISettings
   {
      int Number { get; }

      int GetNumber(

         [Option(Alias = "Azure")]
         string section1Name,
         
         string section2Name);

      void SetNumber(int value);
   }
}
