using System;
using System.Globalization;

namespace Config.Net.TypeParsers
{
   class DoubleParser : ITypeParser
   {
      public bool TryParse(string value, Type t, out object result)
      {
         double dr;
         bool parsed = double.TryParse(value, out dr);
         result = dr;
         return parsed;
         
      }

      public string ToRawString(object value)
      {
         return ((double)value).ToString(TypeParserSettings.DefaultNumericFormat, CultureInfo.InvariantCulture);
      }
   }
}
