using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Config.Net.TypeParsers
{
   class UIntParser : ITypeParser
   {
      public IEnumerable<Type> SupportedTypes => new[] { typeof(uint) };

      public bool TryParse(string value, Type t, out object result)
      {
         uint ir;
         bool parsed = uint.TryParse(value, NumberStyles.Integer, TypeParserSettings.DefaultCulture, out ir);
         result = ir;
         return parsed;
      }

      public string ToRawString(object value)
      {
         return ((uint)value).ToString(TypeParserSettings.DefaultNumericFormat, TypeParserSettings.DefaultCulture);
      }
   }
}
