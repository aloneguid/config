using System;
using System.Globalization;

namespace Config.Net.TypeParsers
{
   class LongParser : ITypeParser
   {
      public bool TryParse(string value, Type t, out object result)
      {
         long lr;
         bool parsed = long.TryParse(value, out lr);
         result = lr;
         return parsed;
      }

      public string ToRawString(object value)
      {
         return ((long)value).ToString(TypeParserSettings.DefaultNumericFormat, CultureInfo.InvariantCulture);
      }
   }
}
