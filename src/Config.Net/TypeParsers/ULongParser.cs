using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Config.Net.TypeParsers
{
   class ULongParser : ITypeParser
   {
      public IEnumerable<Type> SupportedTypes => new[] { typeof(ulong) };

      public bool TryParse(string? value, Type t, out object? result)
      {
         ulong ir;
         bool parsed = ulong.TryParse(value, NumberStyles.Integer, TypeParserSettings.DefaultCulture, out ir);
         result = ir;
         return parsed;
      }

      public string? ToRawString(object? value)
      {
         return ((ulong?)value)?.ToString(TypeParserSettings.DefaultNumericFormat, TypeParserSettings.DefaultCulture);
      }
   }
}
