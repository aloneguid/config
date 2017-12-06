using Config.Net.TypeParsers;
using System.Globalization;
using System.Threading;
using Xunit;

namespace Config.Net.Tests.TypeParsers
{
   public class DoubleParserTest
   {
      private static readonly ITypeParser TypeParser = new DoubleParser();

      [Theory]
      [InlineData("12345")]
      [InlineData("1054.32179")]
      [InlineData("1.797693")]
      [InlineData("-1.797693")]
      public void ToRawString_WhenInputIsValid_ReturnValidString(string rawValue)
      {
         object outValObj;
         double outVal;

         Assert.True(TypeParser.TryParse(rawValue, typeof(double), out outValObj));
         outVal = (double)outValObj;

         string actual = TypeParser.ToRawString(outVal);
         Assert.True(rawValue == actual, $"{rawValue} != {actual}");
      }
   }
}
