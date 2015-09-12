using NUnit.Framework;

namespace Config.Net.Tests.TypeParsers
{
   [TestFixture]
   class IntParserTest
   {
      private static readonly ITypeParser<int> TypeParser = Cfg.Configuration.GetParser<int>();

      [Test]
      [TestCase("1234567890")]
      [TestCase("-1234567890")]
      [TestCase("2147483647")]
      [TestCase("-2147483648")]
      public void ToRawString_WhenInputIsValid_ReturnValidString(string rawValue)
      {
         int outVal;

         Assert.IsTrue(TypeParser.TryParse(rawValue, out outVal));

         Assert.AreEqual(rawValue, TypeParser.ToRawString(outVal));
      }
   }
}
