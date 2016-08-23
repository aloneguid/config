using NUnit.Framework;

namespace Config.Net.Tests.TypeParsers
{
   [TestFixture]
   class StringArrayParserTest
   {
      private static readonly ITypeParser TypeParser = Cfg.Configuration.GetParser(typeof(string[]));

      [Test]
      [TestCase("UK")]
      [TestCase("US")]
      [TestCase("Germany")]
      [TestCase(null)]
      public void ToRawString_WhenInputIsValid_ReturnValidString(string rawValue)
      {
         object outValObj;

         if (rawValue != null)
            Assert.IsTrue(TypeParser.TryParse(rawValue, typeof(string[]), out outValObj));
         else
            Assert.IsFalse(TypeParser.TryParse(null, typeof(string[]), out outValObj));

         Assert.AreEqual(rawValue, TypeParser.ToRawString(outValObj as string[]));
      }
   }
}
