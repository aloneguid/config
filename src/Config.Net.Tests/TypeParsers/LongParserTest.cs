using Config.Net.TypeParsers;
using Xunit;

namespace Config.Net.Tests.TypeParsers
{
   public class LongParserTest
   {
      private static readonly ITypeParser TypeParser = new LongParser();

      [Theory]
      [InlineData("12345")]
      [InlineData("-105479")]
      [InlineData("9223372036854775807")]
      [InlineData("-9223372036854775808")]
      public void ToRawString_WhenInputIsValid_ReturnValidString(string rawValue)
      {
         object outValObj;
         long outVal;

         Assert.True(TypeParser.TryParse(rawValue, typeof(JiraTime), out outValObj));
         outVal = (long)outValObj;

         Assert.Equal(rawValue, TypeParser.ToRawString(outVal));
      }
   }
}
