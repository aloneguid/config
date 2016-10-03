using Config.Net.TypeParsers;
using Xunit;

namespace Config.Net.Tests.TypeParsers
{
   public class StringArrayParserTest
   {
      private static readonly ITypeParser TypeParser = new StringArrayParser();

      [Fact]
      [InlineData("UK")]
      [InlineData("US")]
      [InlineData("Germany")]
      [InlineData(null)]
      public void ToRawString_WhenInputIsValid_ReturnValidString(string rawValue)
      {
         object outValObj;

         if (rawValue != null)
            Assert.True(TypeParser.TryParse(rawValue, typeof(string[]), out outValObj));
         else
            Assert.False(TypeParser.TryParse(null, typeof(string[]), out outValObj));

         Assert.Equal(rawValue, TypeParser.ToRawString(outValObj as string[]));
      }
   }
}
