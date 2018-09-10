using Config.Net.TypeParsers;
using Xunit;

namespace Config.Net.Tests.TypeParsers
{
   public class ULongParserTest
   {
      private static readonly ITypeParser TypeParser = new ULongParser();

      [Theory]
      [InlineData("18446744073709551615")]
      [InlineData("0")]
      [InlineData("12345678901234567890")]
      public void ToRawString_WhenInputIsValid_ReturnValidString(string rawValue)
      {
         object outValObj;
         ulong outVal;

         Assert.True(TypeParser.TryParse(rawValue, typeof(ulong), out outValObj));
         outVal = (ulong)outValObj;

         Assert.Equal(rawValue, TypeParser.ToRawString(outVal));
      }
   }
}
