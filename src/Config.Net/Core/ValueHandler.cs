using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Config.Net.TypeParsers;

namespace Config.Net.Core
{
   class ValueHandler
   {
      private static readonly DefaultParser DefaultParser = new DefaultParser();
      private static readonly ConcurrentDictionary<Type, ITypeParser> Parsers = new ConcurrentDictionary<Type, ITypeParser>();
      private static readonly HashSet<Type> SupportedTypes = new HashSet<Type>();

      private static readonly ValueHandler _default = new ValueHandler();

      public static ValueHandler Default => _default;

      static ValueHandler()
      {
         foreach (ITypeParser pc in GetBuiltInParsers())
         {
            foreach (Type t in pc.SupportedTypes)
            {
               Parsers[t] = pc;
               SupportedTypes.Add(t);
            }
         }
      }

      public static bool IsSupported(Type t)
      {
         return SupportedTypes.Contains(t) || DefaultParser.IsSupported(t);
      }

      public object ParseValue(Type baseType, string rawValue, object defaultValue)
      {
         object result;

         if (rawValue == null)
         {
            result = defaultValue;
         }
         else
         {
            if(!TryParse(baseType, rawValue, out result))
            {
               result = defaultValue;
            }
         }

         return result;
      }

      public bool TryParse(Type propertyType, string rawValue, out object result)
      {
         if (DefaultParser.IsSupported(propertyType))   //type here must be a non-nullable one
         {
            if (!DefaultParser.TryParse(rawValue, propertyType, out result))
            {
               return false;
            }
         }
         else
         {
            ITypeParser typeParser = GetParser(propertyType);
            if (!typeParser.TryParse(rawValue, propertyType, out result))
            {
               return false;
            }
         }

         return true;
      }

      public string ConvertValue(Type baseType, object value)
      {
         string str;

         if (value == null)
         {
            str = null;
         }
         else
         {
            if (DefaultParser.IsSupported(baseType))
            {
               str = DefaultParser.ToRawString(value);
            }
            else
            {
               ITypeParser parser = GetParser(value.GetType());
               str = parser.ToRawString(value);
            }
         }

         return str;
      }

      private ITypeParser GetParser(Type t)
      {
         ITypeParser result;
         Parsers.TryGetValue(t, out result);
         return result;
      }

      /// <summary>
      /// Scans assembly for types implementing <see cref="ITypeParser"/> and builds Type => instance dictionary.
      /// Not sure if I should use reflection here, however the assembly is small and this shouldn't cause any
      /// performance issues
      /// </summary>
      /// <returns></returns>
      private static IEnumerable<ITypeParser> GetBuiltInParsers()
      {
         return new ITypeParser[]
         {
            new DoubleParser(),
            new IntParser(),
            new JiraTimeParser(),
            new LongParser(),
            new StringArrayParser(),
            new StringParser(),
            new TimeSpanParser(),
            new CoreParsers(),
            new NetworkCredentialParser()
         };
      }

   }
}
