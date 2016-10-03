using Config.Net.TypeParsers;
using Xunit;

namespace Config.Net.Tests.TypeParsers
{
   public class IntParserTest
   {
      private static readonly ITypeParser TypeParser = new IntParser();

      [Fact]
      [InlineData("1234567890")]
      [InlineData("-1234567890")]
      [InlineData("2147483647")]
      [InlineData("-2147483648")]
      public void ToRawString_WhenInputIsValid_ReturnValidString(string rawValue)
      {
         object outValObj;
         int outVal;

         Assert.True(TypeParser.TryParse(rawValue, typeof(int), out outValObj));
         outVal = (int)outValObj;

         Assert.Equal(rawValue, TypeParser.ToRawString(outVal));
      }
   }
}
