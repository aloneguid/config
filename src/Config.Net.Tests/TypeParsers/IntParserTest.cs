using NUnit.Framework;

namespace Config.Net.Tests.TypeParsers
{
   [TestFixture]
   class IntParserTest
   {
      private static readonly ITypeParser TypeParser = Cfg.Configuration.GetParser(typeof(int));

      [Test]
      [TestCase("1234567890")]
      [TestCase("-1234567890")]
      [TestCase("2147483647")]
      [TestCase("-2147483648")]
      public void ToRawString_WhenInputIsValid_ReturnValidString(string rawValue)
      {
         object outValObj;
         int outVal;

         Assert.IsTrue(TypeParser.TryParse(rawValue, typeof(int), out outValObj));
         outVal = (int)outValObj;

         Assert.AreEqual(rawValue, TypeParser.ToRawString(outVal));
      }
   }
}
