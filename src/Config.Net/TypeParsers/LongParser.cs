using System;
using System.Collections.Generic;
using System.Globalization;

namespace Config.Net.TypeParsers
{
   class LongParser : ITypeParser
   {
      public IEnumerable<Type> SupportedTypes => new[] { typeof(long) };

      public bool TryParse(string value, Type t, out object result)
      {
         long lr;
         bool parsed = long.TryParse(value, NumberStyles.Integer, TypeParserSettings.DefaultCulture, out lr);
         result = lr;
         return parsed;
      }

      public string ToRawString(object value)
      {
         return ((long)value).ToString(TypeParserSettings.DefaultNumericFormat, TypeParserSettings.DefaultCulture);
      }
   }
}
