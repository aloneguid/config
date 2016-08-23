using System;
using NUnit.Framework;

namespace Config.Net.Tests.TypeParsers
{
   [TestFixture]
   public class DateTimeParserTest
   {
      private static readonly ITypeParser TypeParser = Cfg.Configuration.GetParser(typeof(DateTime));

      [Test]
      public void ParseTwoWays_Variable_Variable()
      {
         var date = DateTime.UtcNow;

         string s = TypeParser.ToRawString(date);

         object date1Obj;
         DateTime date1;
         bool parsed = TypeParser.TryParse(s, typeof(DateTime), out date1Obj);
         date1 = (DateTime)date1Obj;

         Assert.IsTrue(parsed);
         Assert.AreEqual(date.RoundToDay(), date1.RoundToDay());
      }
   }
}
