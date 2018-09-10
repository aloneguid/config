using System;
using System.Collections.Generic;
using System.Text;
using Config.Net.TypeParsers;
using Xunit;

namespace Config.Net.Tests.TypeParsers
{
   public class ByteParserTest
   {
      private static readonly ITypeParser TypeParser = new ByteParser();

      [Theory]
      [InlineData("255")]
      [InlineData("0")]
      [InlineData("128")]
      [InlineData("0x20")]
      public void ToRawString_WhenInputIsValid_ReturnValidString(string rawValue)
      {
         object outValObj;
         byte outVal;

         Assert.True(TypeParser.TryParse(rawValue, typeof(byte), out outValObj));
         outVal = (byte)outValObj;
         if (rawValue.StartsWith("0x"))
         {
            Assert.Equal(rawValue, "0x" + outVal.ToString("X2"));
         }
         else
         {
            Assert.Equal(rawValue, TypeParser.ToRawString(outVal));
         }
      }
   }
}
