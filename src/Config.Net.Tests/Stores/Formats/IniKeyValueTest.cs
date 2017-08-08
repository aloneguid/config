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
      public void FromLine_Variable_Variable(string input, string expectedKey, string expectedValue)
      {
         IniKeyValue kv = IniKeyValue.FromLine(input);

         Assert.Equal(expectedKey, kv.Key);
         Assert.Equal(expectedValue, kv.Value);
      }
   }
}
