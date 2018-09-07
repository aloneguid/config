using System;
using System.Collections.Generic;
using System.Text;
using Config.Net.TypeParsers;
using Xunit;

namespace Config.Net.Tests.TypeParsers
{
   public class UShortParserTest
   {
      private static readonly ITypeParser TypeParser = new UShortParser();

      [Theory]
      [InlineData("65535")]
      [InlineData("0")]
      [InlineData("12345")]
      public void ToRawString_WhenInputIsValid_ReturnValidString(string rawValue)
      {
         object outValObj;
         ushort outVal;

         Assert.True(TypeParser.TryParse(rawValue, typeof(ushort), out outValObj));
         outVal = (ushort)outValObj;

         Assert.Equal(rawValue, TypeParser.ToRawString(outVal));
      }
   }
}
