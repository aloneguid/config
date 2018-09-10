using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Config.Net.TypeParsers
{
   class ByteParser : ITypeParser
   {
      public IEnumerable<Type> SupportedTypes => new[] { typeof(byte) };

      public bool TryParse(string value, Type t, out object result)
      {
         byte ir;
         bool parsed;
         if (value.StartsWith("0x"))
         {
            parsed = byte.TryParse(value.Substring(2), NumberStyles.HexNumber, TypeParserSettings.DefaultCulture, out ir);
         }
         else
         {
            parsed = byte.TryParse(value, NumberStyles.Integer, TypeParserSettings.DefaultCulture, out ir);
         }
         result = ir;
         return parsed;
      }

      public string ToRawString(object value)
      {
         return ((byte)value).ToString(TypeParserSettings.DefaultNumericFormat, TypeParserSettings.DefaultCulture);
      }
   }
}
