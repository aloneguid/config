using System;
using NUnit.Framework;

namespace Config.Net.Tests.TypeParsers
{
   [TestFixture]
   class TimeSpanParserTest
   {
      private static readonly ITypeParser<TimeSpan> TypeParser = Cfg.Configuration.GetParser<TimeSpan>();

      [Test]
      [TestCase("3.00:00:00")]
      [TestCase("00:00:00")]
      [TestCase("23:00:00")]
      [TestCase("00:53:00")]
      [TestCase("00:00:24")]
      [TestCase("01:02:03")]
      [TestCase("00:00:00.1130000")]
      public void ToRawString_WhenInputIsValid_ReturnValidString(string rawValue)
      {
         TimeSpan outVal;

         Assert.IsTrue(TypeParser.TryParse(rawValue, out outVal));

         Assert.AreEqual(rawValue, TypeParser.ToRawString(outVal));
      }
   }
}
