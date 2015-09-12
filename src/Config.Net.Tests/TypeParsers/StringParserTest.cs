using NUnit.Framework;

namespace Config.Net.Tests.TypeParsers
{
   [TestFixture]
   class StringParserTest
   {
      private static readonly ITypeParser<string> TypeParser = Cfg.Configuration.GetParser<string>();

      [Test]
      [TestCase("Aloneguid")]
      [TestCase("")]
      [TestCase(" ")]
      [TestCase(null)]
      public void ToRawString_WhenInputIsValid_ReturnValidString(string rawValue)
      {
         string outVal;

         if(rawValue != null) 
            Assert.IsTrue(TypeParser.TryParse(rawValue, out outVal));
         else
            Assert.IsFalse(TypeParser.TryParse(null, out outVal));
         
         Assert.AreEqual(rawValue, TypeParser.ToRawString(outVal));
      }
   }
}
