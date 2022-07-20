using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Config.Net.TypeParsers
{
   class DecimalParser : ITypeParser
   {
      public IEnumerable<Type> SupportedTypes => new[] { typeof(decimal) };

      public bool TryParse(string? value, Type t, out object? result)
      {
         decimal dr;
         bool parsed = decimal.TryParse(value, NumberStyles.Float, TypeParserSettings.DefaultCulture, out dr);
         result = dr;
         return parsed;

      }

      public string? ToRawString(object? value)
      {
         return ((decimal?)value)?.ToString(TypeParserSettings.DefaultNumericFormat, TypeParserSettings.DefaultCulture);
      }
   }
}
