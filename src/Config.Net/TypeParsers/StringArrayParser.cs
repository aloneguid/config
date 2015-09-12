using System;

namespace Config.Net.TypeParsers
{
   class StringArrayParser : ITypeParser<string[]>
   {
      private static readonly char[] SplitChars = {',', ' '};
      private static readonly string Delimiter = new string(SplitChars);

      public bool TryParse(string value, out string[] result)
      {
         if (value == null)
         {
            result = null;
            return false;
         }

         result = value.Split(SplitChars, StringSplitOptions.RemoveEmptyEntries);
         return true;
      }

      public string ToRawString(string[] value)
      {
         if (value == null || value.Length == 0) return null;

         return string.Join(Delimiter, value);
      }
   }
}
