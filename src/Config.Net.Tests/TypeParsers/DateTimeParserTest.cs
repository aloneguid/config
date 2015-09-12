using System;
using NUnit.Framework;

namespace Config.Net.Tests.TypeParsers
{
   [TestFixture]
   public class DateTimeParserTest
   {
      private static readonly ITypeParser<DateTime> TypeParser = Cfg.Configuration.GetParser<DateTime>();

      [Test]
      public void ParseTwoWays_Variable_Variable()
      {
         var date = DateTime.UtcNow;

         string s = TypeParser.ToRawString(date);

         DateTime date1;
         bool parsed = TypeParser.TryParse(s, out date1);

         Assert.IsTrue(parsed);
         Assert.AreEqual(date.RoundToDay(), date1.RoundToDay());
      }
   }
}
