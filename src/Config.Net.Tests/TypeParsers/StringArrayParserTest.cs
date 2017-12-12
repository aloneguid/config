using Config.Net.TypeParsers;
using Xunit;

namespace Config.Net.Tests.TypeParsers
{
   public class StringArrayParserTest
   {
      private static readonly ITypeParser TypeParser = new StringArrayParser();

      [Theory]
      [InlineData("UK")]
      [InlineData("US")]
      [InlineData("Germany")]
      [InlineData("Germany UK US")]
      [InlineData("\"Germany\" UK US", "Germany UK US")]
      [InlineData("\"Ger ma ny\" UK US", "\"Ger ma ny\" UK US")]
      [InlineData(null)]
      public void ToRawString_WhenInputIsValid_ReturnValidString(string rawValue, string expected = null)
      {
         if (expected == null) expected = rawValue;

         bool parsed = TypeParser.TryParse(rawValue, typeof(string[]), out object outValObj);

         if (rawValue != null)
            Assert.True(parsed);
         else
            Assert.False(parsed);

         Assert.Equal(expected, TypeParser.ToRawString(outValObj as string[]));
      }
   }
}
