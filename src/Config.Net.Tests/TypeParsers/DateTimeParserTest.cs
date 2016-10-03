using System;
using Config.Net.TypeParsers;
using Xunit;

namespace Config.Net.Tests.TypeParsers
{
   public class DateTimeParserTest
   {
      private static readonly ITypeParser TypeParser = new DateTimeParser();

      [Fact]
      public void ParseTwoWays_Variable_Variable()
      {
         var date = DateTime.UtcNow;

         string s = TypeParser.ToRawString(date);

         object date1Obj;
         DateTime date1;
         bool parsed = TypeParser.TryParse(s, typeof(DateTime), out date1Obj);
         Assert.True(parsed);
         Assert.NotNull(date1Obj);

         date1 = (DateTime)date1Obj;
         Assert.True(parsed);
         Assert.Equal(date.RoundToDay(), date1.RoundToDay());
      }
   }
}
