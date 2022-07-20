using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Config.Net.TypeParsers
{
   class UShortParser : ITypeParser
   {
      public IEnumerable<Type> SupportedTypes => new[] { typeof(ushort) };

      public bool TryParse(string? value, Type t, out object? result)
      {
         ushort ir;
         bool parsed = ushort.TryParse(value, NumberStyles.Integer, TypeParserSettings.DefaultCulture, out ir);
         result = ir;
         return parsed;
      }

      public string? ToRawString(object? value)
      {
         return ((ushort?)value)?.ToString(TypeParserSettings.DefaultNumericFormat, TypeParserSettings.DefaultCulture);
      }
   }
}
