using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Config.Net.TypeParsers
{
   class SByteParser : ITypeParser
   {
      public IEnumerable<Type> SupportedTypes => new[] { typeof(sbyte) };

      public bool TryParse(string? value, Type t, out object? result)
      {
         sbyte ir;
         bool parsed = sbyte.TryParse(value, NumberStyles.Integer, TypeParserSettings.DefaultCulture, out ir);
         result = ir;
         return parsed;
      }

      public string? ToRawString(object? value)
      {
         return ((sbyte?)value)?.ToString(TypeParserSettings.DefaultNumericFormat, TypeParserSettings.DefaultCulture);
      }
   }
}
