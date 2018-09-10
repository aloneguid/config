using Config.Net.TypeParsers;
using Xunit;

namespace Config.Net.Tests.TypeParsers
{
   public class FloatParserTest
   {
      private static readonly ITypeParser TypeParser = new FloatParser();

      [Theory]
      [InlineData("1234567")]
      [InlineData("1054.321")]
      [InlineData("0")]
      [InlineData("1.797693")]
      [InlineData("-1.797693")]
      public void ToRawString_WhenInputIsValid_ReturnValidString(string rawValue)
      {
         object outValObj;
         float outVal;

         Assert.True(TypeParser.TryParse(rawValue, typeof(float), out outValObj));
         outVal = (float)outValObj;

         string actual = TypeParser.ToRawString(outVal);
         Assert.True(rawValue == actual, $"{rawValue} != {actual}");
      }
   }
}
