using System;
using System.Collections.Generic;
using System.Text;
using Config.Net.Stores;
using Xunit;

namespace Config.Net.Tests
{
   public class CustomParserTest
   {
      class MyParser : ITypeParser
      {
         public IEnumerable<Type> SupportedTypes => new[] { typeof(byte) };

         public string ToRawString(object value)
         {
            return "byteconst";
         }

         public bool TryParse(string value, Type t, out object result)
         {
            result = (byte)1;

            return true;
         }
      }

      public interface ICustomTypes
      {
         byte Byte { get; set; }
      }

      public interface IStandardTypes
      {
         [Option(DefaultValue = "123")]
         DateTime TheDate { get; set; }
      }

      private IConfigStore _store = new DictionaryConfigStore();

      [Fact]
      public void Custom_byte_parser()
      {
         ICustomTypes config = new ConfigurationBuilder<ICustomTypes>()
            .UseConfigStore(_store)
            .UseTypeParser(new MyParser())
            .Build();

         Assert.Equal((byte)0, config.Byte);

         config.Byte = 3;
         Assert.Equal("byteconst", _store.Read("Byte"));

         Assert.Equal((byte)1, config.Byte);
      }

      [Fact]
      public void Custom_parser_wins_over_builtin_implementation()
      {
         IStandardTypes config = new ConfigurationBuilder<IStandardTypes>()
            .UseTypeParser(new CustomDateParser())
            .Build();

         Assert.Equal(new DateTime(2018, 03, 04), config.TheDate);
      }

      class CustomDateParser : ITypeParser
      {
         public IEnumerable<Type> SupportedTypes => new[] { typeof(DateTime) };

         public string ToRawString(object value)
         {
            return "date";
         }

         public bool TryParse(string value, Type t, out object result)
         {
            result = new DateTime(2018, 03, 04);

            return true;
         }
      }
   }
}
