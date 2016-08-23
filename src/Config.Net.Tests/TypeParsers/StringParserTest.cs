using NUnit.Framework;

namespace Config.Net.Tests.TypeParsers
{
   [TestFixture]
   class StringParserTest
   {
      private static readonly ITypeParser TypeParser = Cfg.Configuration.GetParser(typeof(string));

      [Test]
      [TestCase("Aloneguid")]
      [TestCase("")]
      [TestCase(" ")]
      [TestCase(null)]
      public void ToRawString_WhenInputIsValid_ReturnValidString(string rawValue)
      {
         object outVal;

         if(rawValue != null) 
            Assert.IsTrue(TypeParser.TryParse(rawValue, typeof(string), out outVal));
         else
            Assert.IsFalse(TypeParser.TryParse(null, typeof(string), out outVal));
         
         Assert.AreEqual(rawValue, TypeParser.ToRawString(outVal));
      }
   }
}
