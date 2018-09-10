using System;
using System.Collections.Generic;
using System.Text;
using Config.Net.TypeParsers;
using Xunit;

namespace Config.Net.Tests.TypeParsers
{
   public class ShortParserTest
   {
      private static readonly ITypeParser TypeParser = new ShortParser();

      [Theory]
      [InlineData("32767")]
      [InlineData("-32768")]
      [InlineData("0")]
      [InlineData("20000")]
      [InlineData("-20000")]
      public void ToRawString_WhenInputIsValid_ReturnValidString(string rawValue)
      {
         object outValObj;
         short outVal;

         Assert.True(TypeParser.TryParse(rawValue, typeof(short), out outValObj));
         outVal = (short)outValObj;

         Assert.Equal(rawValue, TypeParser.ToRawString(outVal));
      }
   }
}
