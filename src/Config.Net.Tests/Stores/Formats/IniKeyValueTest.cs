using Config.Net.Stores.Formats.Ini;
using Xunit;

namespace Config.Net.Tests.Stores.Formats
{
   public class IniKeyValueTest
   {
      [Theory]
      [InlineData("key=value", "key", "value", null)]
      [InlineData("key=value;123", "key", "value", "123")]
      [InlineData("key==value", "key", "=value", null)]
      [InlineData("key=value;value;value", "key", "value;value", "value")]
      [InlineData("key=value=value;value", "key", "value=value", "value")]
      [InlineData("key=value;rest;", "key", "value;rest", "")]
      public void FromLine_ParsingInlineComments(string input, string expectedKey, string expectedValue, string expectedComment)
      {
         IniKeyValue kv = IniKeyValue.FromLine(input, true);

         Assert.Equal(expectedKey, kv.Key);
         Assert.Equal(expectedValue, kv.Value);
         
         if (expectedComment == null)
         {
            Assert.Null(kv.Comment);
         }
         else
         {
            Assert.Equal(expectedComment, kv.Comment.Value);
         }
      }

      [Theory]
      [InlineData("key=value", "key", "value")]
      [InlineData("key=value;123", "key", "value;123")]
      [InlineData("key==value", "key", "=value")]
      [InlineData("key=value;value;value", "key", "value;value;value")]
      [InlineData("key=value=value;value", "key", "value=value;value")]
      public void FromLine_IgnoringInlineComments(string input, string expectedKey, string expectedValue)
      {
         IniKeyValue kv = IniKeyValue.FromLine(input, false);

         Assert.Equal(expectedKey, kv.Key);
         Assert.Equal(expectedValue, kv.Value);
         Assert.Null(kv.Comment);
      }
   }
}
