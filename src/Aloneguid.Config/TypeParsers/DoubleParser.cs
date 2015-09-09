using System.Globalization;

namespace Aloneguid.Config.TypeParsers
{
   class DoubleParser : ITypeParser<double>
   {
      public bool TryParse(string value, out double result)
      {
         return double.TryParse(value, out result);
      }

      public string ToRawString(double value)
      {
         return value.ToString(TypeParserSettings.DefaultNumericFormat, CultureInfo.InvariantCulture);
      }
   }
}
