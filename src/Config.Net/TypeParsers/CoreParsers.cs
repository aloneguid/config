using System;
using System.Collections.Generic;

namespace Config.Net.TypeParsers
{
   /// <summary>
   /// Container for core types. Eventually all simple parsers will merge into this class.
   /// </summary>
   class CoreParsers : ITypeParser
   {
      public IEnumerable<Type> SupportedTypes => new[] { typeof(Uri), typeof(bool) };

      private static readonly Dictionary<string, bool> BooleanTrueValues =
         new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase)
      {
         {"true", true},
         {"yes", true},
         {"1", true},
      };


      public string ToRawString(object value)
      {
         if (value == null) return null;

         Type t = value.GetType();

         if(t == typeof(Uri))
            return value.ToString();

         if(t == typeof(bool))
            return value.ToString().ToLowerInvariant();

         return null;
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

         if(t == typeof(bool))
         {
            if(BooleanTrueValues.ContainsKey(value))
            {
               result = true;
               return true;
            }

            result = false;
            return true;
         }

         result = null;
         return false;
      }
   }
}
