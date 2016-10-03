using Config.Net.TypeParsers;
using Xunit;

namespace Config.Net.Tests.TypeParsers
{
   public class BoolParserTest
   {
      private static readonly ITypeParser TypeParser = new BoolParser();

      [Fact]
      [InlineData("true")]
      [InlineData("false")]
      [InlineData("yes")]
      [InlineData("no")]
      [InlineData("1")]
      [InlineData("0")]
      public void ToRawString_WhenInputIsValid_ReturnValidString(string rawValue)
      {
         object outValObj;
         bool outVal;

         Assert.True(TypeParser.TryParse(rawValue, typeof(bool), out outValObj));
         outVal = (bool)outValObj;

         //ToRawString does not handle cases for yes/no/1/0
         if(rawValue.Equals("true") || rawValue.Equals("false")) 
            Assert.Equal(rawValue, TypeParser.ToRawString(outVal));
         else
            Assert.NotEqual(rawValue, TypeParser.ToRawString(outVal));
      }
   }
}
