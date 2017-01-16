using System;
using System.Collections.Generic;

namespace Config.Net.TypeParsers
{
   /// <summary>
   /// Container for core types. Eventually all simple parsers will merge into this class.
   /// </summary>
   class CoreParsers : ITypeParser
   {
      public IEnumerable<Type> SupportedTypes => new[] { typeof(Uri) };

      public string ToRawString(object value)
      {
         if (value == null) return null;

         return value.ToString();
      }

      public bool TryParse(string value, Type t, out object result)
      {
         if(value == null)
         {
            result = null;
            return false;
         }

         if(t == typeof(Uri))
         {
            Uri uri = new Uri(value);
            result = uri;
            return true;
         }

         result = null;
         return false;
      }
   }
}
