using Config.Net.TypeParsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Config.Net.Tests
{
   public class CoreParsersTest
   {
      private static readonly ITypeParser Parser = new CoreParsers();

      [Theory]
      [InlineData("true", "true")]
      [InlineData("false", "false")]
      [InlineData("yes", "true")]
      [InlineData("no", "false")]
      [InlineData("1", "true")]
      [InlineData("0", "false")]
      public void Boolean__(string input, string expected)
      {
         object result;
         bool parsed = Parser.TryParse(input, typeof(bool), out result);
         Assert.True(parsed); // boolean always parses

         string back = Parser.ToRawString(result);
         Assert.Equal(expected, back);
      }

      [Fact]
      public void Guid_Valid_Parses()
      {
         Guid g = Guid.NewGuid();

         bool parsed = Parser.TryParse(g.ToString(), typeof(Guid), out object result);

         Assert.True(parsed);
         string back = Parser.ToRawString(g);
         Assert.Equal(g.ToString(), back);
      }

      [Fact]
      public void Guid_Invalid_False()
      {
         bool parsed = Parser.TryParse("dsfdsf", typeof(Guid), out object result);
         Assert.False(parsed);
         Assert.Null(result);
      }
   }
}
