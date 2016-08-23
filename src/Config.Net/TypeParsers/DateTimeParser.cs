using System;
using System.Collections.Generic;

namespace Config.Net.TypeParsers
{
   class DateTimeParser : ITypeParser
   {
      public IEnumerable<Type> SupportedTypes => new[] { typeof(DateTime) };

      public bool TryParse(string value, out DateTime result)
      {
         if(value == null)
         {
            result = DateTime.MinValue;
            return false;
         }

         return DateTime.TryParse(value, out result);
      }

      public string ToRawString(DateTime value)
      {
         return value.ToString("u");
      }

      public bool TryParse(string value, Type t, out object result)
      {
         throw new NotImplementedException();
      }

      public string ToRawString(object value)
      {
         throw new NotImplementedException();
      }
   }
}
