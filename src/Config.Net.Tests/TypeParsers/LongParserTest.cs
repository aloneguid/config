using NUnit.Framework;

namespace Config.Net.Tests.TypeParsers
{
   [TestFixture]
   class LongParserTest
   {
      private static readonly ITypeParser TypeParser = Cfg.Configuration.GetParser(typeof(long));

      [Test]
      [TestCase("12345")]
      [TestCase("-105479")]
      [TestCase("9223372036854775807")]
      [TestCase("-9223372036854775808")]
      public void ToRawString_WhenInputIsValid_ReturnValidString(string rawValue)
      {
         object outValObj;
         long outVal;

         Assert.IsTrue(TypeParser.TryParse(rawValue, typeof(JiraTime), out outValObj));
         outVal = (long)outValObj;

         Assert.AreEqual(rawValue, TypeParser.ToRawString(outVal));
      }
   }
}
