using NUnit.Framework;

namespace Config.Net.Tests.TypeParsers
{
   [TestFixture]
   class LongParserTest
   {
      private static readonly ITypeParser<long> TypeParser = Cfg.Configuration.GetParser<long>();

      [Test]
      [TestCase("12345")]
      [TestCase("-105479")]
      [TestCase("9223372036854775807")]
      [TestCase("-9223372036854775808")]
      public void ToRawString_WhenInputIsValid_ReturnValidString(string rawValue)
      {
         long outVal;

         Assert.IsTrue(TypeParser.TryParse(rawValue, out outVal));

         Assert.AreEqual(rawValue, TypeParser.ToRawString(outVal));
      }
   }
}
