using Config.Net.Stores.Formats.Ini;
using Xunit;

namespace Config.Net.Tests.Stores.Formats
{
   public class IniKeyValueTest
   {
      [Theory]
      [InlineData("key=value", "key", "value")]
      [InlineData("key=value;123", "key", "value")]
      [InlineData("key==value", "key", "=value")]
      [InlineData("key=value;value;value", "key", "value;value")]
      [InlineData("key=value=value;value", "key", "value=value")]
      public void FromLine_ParsingInlineComments(string input, string expectedKey, string expectedValue)
      {
         IniKeyValue kv = IniKeyValue.FromLine(input, true);

         Assert.Equal(expectedKey, kv.Key);
         Assert.Equal(expectedValue, kv.Value);
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
      }

   }
}
