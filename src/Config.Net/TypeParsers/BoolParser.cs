using System;
using System.Collections.Generic;
using System.Text;

namespace Config.Net.TypeParsers
{
   class BoolParser : ITypeParser
   {
      public IEnumerable<Type> SupportedTypes => new[] { typeof(bool) };

      public bool TryParse(string value, Type t, out object result)
      {
         bool ir;
         bool parsed = bool.TryParse(value, out ir);
         result = ir;
         return parsed;
      }

      public string ToRawString(object value)
      {
         return ((bool)value).ToString();
      }
   }
}
