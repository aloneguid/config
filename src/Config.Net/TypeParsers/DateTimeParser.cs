using System;
using System.Collections.Generic;

namespace Config.Net.TypeParsers
{
   class DateTimeParser : ITypeParser
   {
      public IEnumerable<Type> SupportedTypes => new[] { typeof(DateTime) };

      public bool TryParse(string value, Type t, out object result)
      {
         if(value == null)
         {
            result = DateTime.MinValue;
            return false;
         }

         DateTime dateResult;
         bool parsed = DateTime.TryParse(value, out dateResult);
         result = dateResult;
         return parsed;
      }

      public string ToRawString(object value)
      {
         return ((DateTime)value).ToString("u");
      }
   }
}
