using NUnit.Framework;

namespace Config.Net.Tests.TypeParsers
{
   [TestFixture]
   class DoubleParserTest
   {
      private static readonly ITypeParser<double> TypeParser = Cfg.Configuration.GetParser<double>();

      [Test]
      [TestCase("12345")]
      [TestCase("1054.32179")]
      [TestCase("1.797693")]
      [TestCase("-1.797693")]
      public void ToRawString_WhenInputIsValid_ReturnValidString(string rawValue)
      {
         double outVal;

         Assert.IsTrue(TypeParser.TryParse(rawValue, out outVal));

         Assert.AreEqual(rawValue, TypeParser.ToRawString(outVal));
      }
   }
}
