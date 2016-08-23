using System;
using System.Collections.Generic;

namespace Config.Net.TypeParsers
{
   class StringArrayParser : ITypeParser
   {
      private static readonly char[] SplitChars = {',', ' '};
      private static readonly string Delimiter = new string(SplitChars);

      public IEnumerable<Type> SupportedTypes => new[] { typeof(string[]) };

      public bool TryParse(string value, Type t, out object result)
      {
         if (value == null)
         {
            result = null;
            return false;
         }

         result = value.Split(SplitChars, StringSplitOptions.RemoveEmptyEntries);
         return true;
      }

      public string ToRawString(object value)
      {
         string[] arv = (string[])value;

         if (arv == null || arv.Length == 0) return null;

         return string.Join(Delimiter, arv);
      }
   }
}
