using System.Globalization;

namespace Aloneguid.Config.TypeParsers
{
   class IntParser : ITypeParser<int>
   {
      public bool TryParse(string value, out int result)
      {
         return int.TryParse(value, out result);
      }

      public string ToRawString(int value)
      {
         return value.ToString(TypeParserSettings.DefaultNumericFormat, CultureInfo.InvariantCulture);
      }
   }
}
