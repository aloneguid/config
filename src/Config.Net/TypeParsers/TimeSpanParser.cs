using System;
using System.Collections.Generic;

namespace Config.Net.TypeParsers
{
   class TimeSpanParser : ITypeParser
   {
      public IEnumerable<Type> SupportedTypes => new[] { typeof(TimeSpan) };

      public bool TryParse(string value, Type t, out object result)
      {
         TimeSpan ts;
         bool parsed = TimeSpan.TryParse(value, TypeParserSettings.DefaultCulture, out ts);
         result = ts;
         return parsed;
      }

      public string ToRawString(object value)
      {
         return ((TimeSpan)value).ToString(null, TypeParserSettings.DefaultCulture);
      }
   }
}
