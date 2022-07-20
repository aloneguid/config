using System;
using System.Collections.Generic;
using System.Globalization;

namespace Config.Net.TypeParsers
{
   /// <summary>
   /// Container for core types. Eventually all simple parsers will merge into this class.
   /// </summary>
   class CoreParsers : ITypeParser
   {
      public IEnumerable<Type> SupportedTypes => new[] { typeof(Uri), typeof(bool), typeof(Guid), typeof(DateTime) };

      private static readonly Dictionary<string, bool> BooleanTrueValues =
         new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase)
      {
         {"true", true},
         {"yes", true},
         {"1", true},
      };


      public string? ToRawString(object? value)
      {
         if (value == null) return null;

         Type t = value.GetType();

         if(t == typeof(Uri))
            return value.ToString();

         if(t == typeof(bool))
            return value?.ToString()?.ToLowerInvariant();

         if (t == typeof(Guid))
            return value.ToString();

         if(t == typeof(DateTime))
            return ((DateTime)value).ToUniversalTime().ToString("u");

         return null;
      }

      public bool TryParse(string? value, Type t, out object? result)
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

         if(t == typeof(Guid))
         {
            if(Guid.TryParse(value, out Guid tg))
            {
               result = tg;
               return true;
            }

            result = null;
            return false;
         }

         if(t == typeof(DateTime))
         {
            DateTime dateResult;
            bool parsed = DateTime.TryParse(value, TypeParserSettings.DefaultCulture, DateTimeStyles.None, out dateResult);
            result = dateResult;
            return parsed;
         }

         result = null;
         return false;
      }
   }
}
