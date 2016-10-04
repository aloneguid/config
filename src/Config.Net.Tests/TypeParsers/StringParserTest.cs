using Config.Net.TypeParsers;
using Xunit;

namespace Config.Net.Tests.TypeParsers
{
   public class StringParserTest
   {
      private static readonly ITypeParser TypeParser = new StringParser();

      [Theory]
      [InlineData("Aloneguid")]
      [InlineData("")]
      [InlineData(" ")]
      [InlineData(null)]
      public void ToRawString_WhenInputIsValid_ReturnValidString(string rawValue)
      {
         object outVal;

         if(rawValue != null) 
            Assert.True(TypeParser.TryParse(rawValue, typeof(string), out outVal));
         else
            Assert.False(TypeParser.TryParse(null, typeof(string), out outVal));
         
         Assert.Equal(rawValue, TypeParser.ToRawString(outVal));
      }
   }
}
