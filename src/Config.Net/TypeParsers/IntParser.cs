using System;
using System.Globalization;

namespace Config.Net.TypeParsers
{
   class IntParser : ITypeParser
   {
      public bool TryParse(string value, Type t, out object result)
      {
         int ir;
         bool parsed = int.TryParse(value, out ir);
         result = ir;
         return parsed;
      }

      public string ToRawString(object value)
      {
         return ((int)value).ToString(TypeParserSettings.DefaultNumericFormat, CultureInfo.InvariantCulture);
      }
   }
}
