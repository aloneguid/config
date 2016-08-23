using System;
using System.Collections.Generic;

namespace Config.Net.TypeParsers
{
   class StringParser : ITypeParser
   {
      public IEnumerable<Type> SupportedTypes => new[] { typeof(string) };

      public bool TryParse(string value, Type t, out object result)
      {
         result = value;
         return value != null;
      }

      public string ToRawString(object value)
      {
         return value as string;
      }
   }
}
