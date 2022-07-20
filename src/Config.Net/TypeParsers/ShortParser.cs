using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Config.Net.TypeParsers
{
   class ShortParser : ITypeParser
   {
      public IEnumerable<Type> SupportedTypes => new[] { typeof(short) };

      public bool TryParse(string? value, Type t, out object? result)
      {
         short ir;
         bool parsed = short.TryParse(value, NumberStyles.Integer, TypeParserSettings.DefaultCulture, out ir);
         result = ir;
         return parsed;
      }

      public string? ToRawString(object? value)
      {
         return ((short?)value)?.ToString(TypeParserSettings.DefaultNumericFormat, TypeParserSettings.DefaultCulture);
      }
   }
}
