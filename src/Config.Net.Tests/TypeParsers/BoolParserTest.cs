using NUnit.Framework;

namespace Config.Net.Tests.TypeParsers
{
   [TestFixture]
   class BoolParserTest
   {
      private static readonly ITypeParser<bool> TypeParser = Cfg.Configuration.GetParser<bool>();

      [Test]
      [TestCase("true")]
      [TestCase("false")]
      [TestCase("yes")]
      [TestCase("no")]
      [TestCase("1")]
      [TestCase("0")]
      public void ToRawString_WhenInputIsValid_ReturnValidString(string rawValue)
      {
         bool outVal;

         Assert.IsTrue(TypeParser.TryParse(rawValue, out outVal));

         //ToRawString does not handle cases for yes/no/1/0
         if(rawValue.Equals("true") || rawValue.Equals("false")) 
            Assert.AreEqual(rawValue, TypeParser.ToRawString(outVal));
         else
            Assert.AreNotEqual(rawValue, TypeParser.ToRawString(outVal));
      }
   }
}
