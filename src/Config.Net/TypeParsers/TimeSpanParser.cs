using System;

namespace Config.Net.TypeParsers
{
   class TimeSpanParser : ITypeParser<TimeSpan>
   {
      public bool TryParse(string value, out TimeSpan result)
      {
         return TimeSpan.TryParse(value, out result);
      }

      public string ToRawString(TimeSpan value)
      {
         return value.ToString();
      }
   }
}
