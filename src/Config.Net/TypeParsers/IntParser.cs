using System;
using System.Collections.Generic;
using System.Globalization;

namespace Config.Net.TypeParsers
{
   class IntParser : ITypeParser
   {
      public IEnumerable<Type> SupportedTypes => new[] { typeof(int) };

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
