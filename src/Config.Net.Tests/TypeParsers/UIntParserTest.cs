using System;
using System.Collections.Generic;
using System.Text;
using Config.Net.TypeParsers;
using Xunit;

namespace Config.Net.Tests.TypeParsers
{
   public class UIntParserTest
   {
      private static readonly ITypeParser TypeParser = new UIntParser();

      [Theory]
      [InlineData("4294967295")]
      [InlineData("0")]
      [InlineData("1234567890")]
      public void ToRawString_WhenInputIsValid_ReturnValidString(string rawValue)
      {
         object outValObj;
         uint outVal;

         Assert.True(TypeParser.TryParse(rawValue, typeof(uint), out outValObj));
         outVal = (uint)outValObj;

         Assert.Equal(rawValue, TypeParser.ToRawString(outVal));
      }
   }
}
