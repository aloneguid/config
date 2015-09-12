using System.Globalization;

namespace Config.Net.TypeParsers
{
   class LongParser : ITypeParser<long>
   {
      public bool TryParse(string value, out long result)
      {
         return long.TryParse(value, out result);
      }

      public string ToRawString(long value)
      {
         return value.ToString(TypeParserSettings.DefaultNumericFormat, CultureInfo.InvariantCulture);
      }
   }
}
