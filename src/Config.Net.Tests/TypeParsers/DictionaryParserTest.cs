using Config.Net.TypeParsers;
using System.Collections.Generic;
using Xunit;

namespace Config.Net.Tests.TypeParsers
{
   public class DictionaryParserTest
   {
      private static readonly ITypeParser TypeParser = new DictionaryParser();

      [Theory]
      [InlineData("{\"a\":\"aval\",\"b\":\"bval\"}")]
      [InlineData("{\"a\":\"aval\"}")]
      [InlineData(null)]
      public void ToRawString_WhenInputIsValid_ReturnValidString(string rawValue)
      {
         object outValObj;

         if (rawValue != null)
            Assert.True(TypeParser.TryParse(rawValue, typeof(Dictionary<string, string>), out outValObj));
         else
            Assert.False(TypeParser.TryParse(null, typeof(Dictionary<string, string>), out outValObj));

         Assert.Equal(rawValue, TypeParser.ToRawString(outValObj as Dictionary<string, string>));
      }
   }
}
