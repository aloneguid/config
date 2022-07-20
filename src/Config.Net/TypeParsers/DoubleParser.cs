using System;
using System.Collections.Generic;
using System.Globalization;

namespace Config.Net.TypeParsers
{
   class DoubleParser : ITypeParser
   {
      public IEnumerable<Type> SupportedTypes => new[] { typeof(double) };

      public bool TryParse(string? value, Type t, out object? result)
      {
         double dr;
         bool parsed = double.TryParse(value, NumberStyles.Float, TypeParserSettings.DefaultCulture, out dr);
         result = dr;
         return parsed;
         
      }

      public string? ToRawString(object? value)
      {
         return ((double?)value)?.ToString(TypeParserSettings.DefaultNumericFormat, TypeParserSettings.DefaultCulture);
      }
   }
}
