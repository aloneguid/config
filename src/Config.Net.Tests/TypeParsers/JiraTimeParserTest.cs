using Config.Net.TypeParsers;
using Xunit;

namespace Config.Net.Tests.TypeParsers
{
   public class JiraTimeParserTest
   {
      private static readonly ITypeParser TypeParser = new JiraTimeParser();

      [Theory]
      [InlineData("3d")]
      [InlineData("23h")]
      [InlineData("53m")]
      [InlineData("24s")]
      [InlineData("3ms")]
      [InlineData("1d2h3m4s")]
      [InlineData(null)]
      public void ToRawString_WhenInputIsValid_ReturnValidString(string rawValue)
      {
         object outValObj;

         if (rawValue != null)
            Assert.True(TypeParser.TryParse(rawValue, typeof(JiraTime), out outValObj));
         else
            Assert.False(TypeParser.TryParse(null, typeof(JiraTime), out outValObj));

         Assert.Equal(rawValue, TypeParser.ToRawString(outValObj));
      }

      [Theory]
      [InlineData("8d")]
      [InlineData("3h")]
      [InlineData("4m")]
      [InlineData("5s")]
      [InlineData("10ms")]
      [InlineData("8d9h")]
      [InlineData("8d7m")]
      [InlineData("8d4ms")]
      [InlineData("8h9m")]
      [InlineData("8h7s")]
      [InlineData("8h4ms")]
      [InlineData("8m9s")]
      [InlineData("8m7ms")]
      [InlineData("3s7ms")]
      [InlineData("1d23h49m52s89ms")]
      [InlineData("")]
      public void TryParse_ValidValue_ReturnsTrue(string rawValue)
      {
         object outValObj;
         JiraTime outVal;
         bool result = TypeParser.TryParse(rawValue, typeof(JiraTime), out outValObj);
         outVal = (JiraTime)outValObj;

         Assert.True(result);
      }

      [Theory]
      [InlineData("8dg")]
      [InlineData("5ms3d")]
      [InlineData("3hg")]
      [InlineData("4mg")]
      [InlineData("5sg")]
      [InlineData("10msg")]
      [InlineData("8dg9hg")]
      [InlineData("8dg7mg")]
      [InlineData("8dg4msg")]
      [InlineData("8hg9mg")]
      [InlineData("8hg7sg")]
      [InlineData("8hg4msg")]
      [InlineData("8mg9sg")]
      [InlineData("8mg7msg")]
      [InlineData("3sg7msg")]
      [InlineData("blah")]
      [InlineData("1d5y")]
      [InlineData("1d 5h 6m 8s")]
      [InlineData("1d 5h 8m 6s 12ms")]
      [InlineData("1d5w")]
      [InlineData("8days")]
      [InlineData("3hours")]
      [InlineData("4minutes")]
      [InlineData("5seconds")]
      [InlineData("10milliseconds")]
      [InlineData("8days9hours")]
      [InlineData("8days7minutes")]
      [InlineData("8days4milliseconds")]
      [InlineData("8hours9minutes")]
      [InlineData("8hours7seconds")]
      [InlineData("8hours4milliseconds")]
      [InlineData("8minutes9seconds")]
      [InlineData("8minutes7milliseconds")]
      [InlineData("3seconds7milliseconds")]
      [InlineData(null)]
      public void TryParse_InValidValue_ReturnsFalse(string rawValue)
      {
         object outValObj;
         bool result = TypeParser.TryParse(rawValue, typeof(JiraTime), out outValObj);

         Assert.False(result);
      }
   }
}