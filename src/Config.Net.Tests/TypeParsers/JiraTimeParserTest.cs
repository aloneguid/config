using NUnit.Framework;

namespace Config.Net.Tests.TypeParsers
{
   [TestFixture]
   class JiraTimeParserTest
   {
      private static readonly ITypeParser<JiraTime> TypeParser = Cfg.Configuration.GetParser<JiraTime>();

      [Test]
      [TestCase("3d")]
      [TestCase("23h")]
      [TestCase("53m")]
      [TestCase("24s")]
      [TestCase("3ms")]
      [TestCase("1d2h3m4s")]
      [TestCase(null)]
      public void ToRawString_WhenInputIsValid_ReturnValidString(string rawValue)
      {
         JiraTime outVal;

         if (rawValue != null)
            Assert.IsTrue(TypeParser.TryParse(rawValue, out outVal));
         else
            Assert.IsFalse(TypeParser.TryParse(null, out outVal));

         Assert.AreEqual(rawValue, TypeParser.ToRawString(outVal));
      }

      [Test]
      [TestCase("8d")]
      [TestCase("3h")]
      [TestCase("4m")]
      [TestCase("5s")]
      [TestCase("10ms")]
      [TestCase("8d9h")]
      [TestCase("8d7m")]
      [TestCase("8d4ms")]
      [TestCase("8h9m")]
      [TestCase("8h7s")]
      [TestCase("8h4ms")]
      [TestCase("8m9s")]
      [TestCase("8m7ms")]
      [TestCase("3s7ms")]
      [TestCase("1d23h49m52s89ms")]
      [TestCase("")]
      public void TryParse_ValidValue_ReturnsTrue(string rawValue)
      {
         JiraTime outVal;
         bool result = TypeParser.TryParse(rawValue, out outVal);

         Assert.IsTrue(result);
      }

      [Test]
      [TestCase("8dg")]
      [TestCase("5ms3d")]
      [TestCase("3hg")]
      [TestCase("4mg")]
      [TestCase("5sg")]
      [TestCase("10msg")]
      [TestCase("8dg9hg")]
      [TestCase("8dg7mg")]
      [TestCase("8dg4msg")]
      [TestCase("8hg9mg")]
      [TestCase("8hg7sg")]
      [TestCase("8hg4msg")]
      [TestCase("8mg9sg")]
      [TestCase("8mg7msg")]
      [TestCase("3sg7msg")]
      [TestCase("blah")]
      [TestCase("1d5y")]
      [TestCase("1d 5h 6m 8s")]
      [TestCase("1d 5h 8m 6s 12ms")]
      [TestCase("1d5w")]
      [TestCase("8days")]
      [TestCase("3hours")]
      [TestCase("4minutes")]
      [TestCase("5seconds")]
      [TestCase("10milliseconds")]
      [TestCase("8days9hours")]
      [TestCase("8days7minutes")]
      [TestCase("8days4milliseconds")]
      [TestCase("8hours9minutes")]
      [TestCase("8hours7seconds")]
      [TestCase("8hours4milliseconds")]
      [TestCase("8minutes9seconds")]
      [TestCase("8minutes7milliseconds")]
      [TestCase("3seconds7milliseconds")]
      [TestCase(null)]
      public void TryParse_InValidValue_ReturnsFalse(string rawValue)
      {
         JiraTime outVal;
         bool result = TypeParser.TryParse(rawValue, out outVal);

         Assert.IsFalse(result);
      }
   }
}