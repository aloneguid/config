using System.Globalization;

namespace Config.Net.TypeParsers
{
   internal static class TypeParserSettings
   {
      public const string DefaultNumericFormat = "G";

      public static readonly CultureInfo DefaultCulture = CultureInfo.InvariantCulture;
   }
}
