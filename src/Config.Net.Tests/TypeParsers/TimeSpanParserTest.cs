using System;
using Config.Net.TypeParsers;
using Xunit;

namespace Config.Net.Tests.TypeParsers
{
   public class TimeSpanParserTest
   {
      private static readonly ITypeParser TypeParser = new TimeSpanParser();

      [Theory]
      [InlineData("3.00:00:00")]
      [InlineData("00:00:00")]
      [InlineData("23:00:00")]
      [InlineData("00:53:00")]
      [InlineData("00:00:24")]
      [InlineData("01:02:03")]
      [InlineData("00:00:00.1130000")]
      public void ToRawString_WhenInputIsValid_ReturnValidString(string rawValue)
      {
         object outValObj;
         TimeSpan outVal;

         Assert.True(TypeParser.TryParse(rawValue, typeof(TimeSpan), out outValObj));
         outVal = (TimeSpan)outValObj;

         Assert.Equal(rawValue, TypeParser.ToRawString(outVal));
      }
   }
}
