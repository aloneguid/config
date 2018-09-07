using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Config.Net.TypeParsers
{
   class FloatParser : ITypeParser
   {
      public IEnumerable<Type> SupportedTypes => new[] { typeof(float) };

      public bool TryParse(string value, Type t, out object result)
      {
         float dr;
         bool parsed = float.TryParse(value, NumberStyles.Float, TypeParserSettings.DefaultCulture, out dr);
         result = dr;
         return parsed;

      }

      public string ToRawString(object value)
      {
         return ((float)value).ToString(TypeParserSettings.DefaultNumericFormat, TypeParserSettings.DefaultCulture);
      }
   }
}
