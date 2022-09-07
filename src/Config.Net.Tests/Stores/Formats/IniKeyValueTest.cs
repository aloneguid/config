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
         FromLine_DoTest(true, false, input, expectedKey, expectedValue, expectedComment);
      }

      [Theory]
      [InlineData("key=value", "key", "value")]
      [InlineData("key=value;123", "key", "value;123")]
      [InlineData("key==value", "key", "=value")]
      [InlineData("key=value;value;value", "key", "value;value;value")]
      [InlineData("key=value=value;value", "key", "value=value;value")]
      public void FromLine_IgnoringInlineComments(string input, string expectedKey, string expectedValue)
      {
         FromLine_DoTest(false, false, input, expectedKey, expectedValue, null);
      }

      [Theory]
      [InlineData(true, @"k1\r\nk2=v1\r\nv2;c1\r\nc2", "k1\r\nk2", "v1\r\nv2", "c1\r\nc2")]
      [InlineData(false, @"k1\r\nk2=v1\r\nv2;c1\r\nc2", @"k1\r\nk2", @"v1\r\nv2", @"c1\r\nc2")]
      public void FromLine_UnescapeNewLines(bool unescapeNewLines, string input, string expectedKey, string expectedValue, string expectedComment)
      {
         FromLine_DoTest(true, unescapeNewLines, input, expectedKey, expectedValue, expectedComment);
      }

      private static void FromLine_DoTest(bool parseInlineComments, bool unescapeNewLines, string input, string expectedKey, string expectedValue, string expectedComment)
      {
         IniKeyValue kv = IniKeyValue.FromLine(input, parseInlineComments, unescapeNewLines);

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
   }
}
