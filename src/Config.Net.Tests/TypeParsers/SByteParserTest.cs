using Config.Net.TypeParsers;
using Xunit;

namespace Config.Net.Tests.TypeParsers
{
   public class SByteParserTest
   {
      private static readonly ITypeParser TypeParser = new SByteParser();

      [Theory]
      [InlineData("127")]
      [InlineData("-128")]
      [InlineData("0")]
      [InlineData("123")]
      public void ToRawString_WhenInputIsValid_ReturnValidString(string rawValue)
      {
         object outValObj;
         sbyte outVal;

         Assert.True(TypeParser.TryParse(rawValue, typeof(sbyte), out outValObj));
         outVal = (sbyte)outValObj;

         Assert.Equal(rawValue, TypeParser.ToRawString(outVal));
      }
   }
}
