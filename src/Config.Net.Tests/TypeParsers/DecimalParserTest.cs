using Config.Net.TypeParsers;
using Xunit;

namespace Config.Net.Tests.TypeParsers
{
   public class DecimalParserTest
   {
      private static readonly ITypeParser TypeParser = new DecimalParser();

      [Theory]
      [InlineData("1234567890.123456789012345678")]
      [InlineData("-1234567890.123456789012345678")]
      [InlineData("0")]
      [InlineData("1.797693")]
      [InlineData("-1.797693")]
      public void ToRawString_WhenInputIsValid_ReturnValidString(string rawValue)
      {
         object outValObj;
         decimal outVal;

         Assert.True(TypeParser.TryParse(rawValue, typeof(decimal), out outValObj));
         outVal = (decimal)outValObj;

         string actual = TypeParser.ToRawString(outVal);
         Assert.True(rawValue == actual, $"{rawValue} != {actual}");
      }
   }
}
