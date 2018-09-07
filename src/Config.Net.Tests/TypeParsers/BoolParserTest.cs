using Config.Net.TypeParsers;
using Xunit;

namespace Config.Net.Tests.TypeParsers
{
   public class BoolParserTest
   {
      private static readonly ITypeParser TypeParser = new BoolParser();

      [Theory]
      [InlineData("True")]
      [InlineData("False")]
      [InlineData("TRUE")]
      [InlineData("FALSE")]
      [InlineData("true")]
      [InlineData("false")]
      public void ToRawString_WhenInputIsValid_ReturnValidString(string rawValue)
      {
         object outValObj;
         bool outVal;

         Assert.True(TypeParser.TryParse(rawValue, typeof(bool), out outValObj));
         outVal = (bool)outValObj;

         Assert.Equal(rawValue.ToLower(), TypeParser.ToRawString(outVal).ToLower());
      }
   }
}
