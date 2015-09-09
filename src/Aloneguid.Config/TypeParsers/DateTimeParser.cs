using System;

namespace Aloneguid.Config.TypeParsers
{
   class DateTimeParser : ITypeParser<DateTime>
   {
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
   }
}
