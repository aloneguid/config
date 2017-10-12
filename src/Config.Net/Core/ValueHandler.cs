using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Config.Net.TypeParsers;

namespace Config.Net.Core
{
   class ValueHandler
   {
      private readonly DefaultParser _defaultParser = new DefaultParser();
      private readonly ConcurrentDictionary<Type, ITypeParser> _parsers = new ConcurrentDictionary<Type, ITypeParser>();

      private static readonly ValueHandler _default = new ValueHandler();

      public static ValueHandler Default => _default;

      private ValueHandler()
      {
         foreach (ITypeParser pc in GetBuiltInParsers())
         {
            foreach (Type t in pc.SupportedTypes)
            {
               _parsers[t] = pc;
            }
         }
      }

      public object ParseValue(PropertyOptions property, string rawValue)
      {
         object result;

         if (rawValue == null)
         {
            result = property.DefaultValue;
         }
         else
         {
            if(!TryParse(property.BaseType, rawValue, out result))
            {
               result = property.DefaultValue;
            }
         }

         return result;
      }

      public bool TryParse(Type propertyType, string rawValue, out object result)
      {
         if (_defaultParser.IsSupported(propertyType))   //type here must be a non-nullable one
         {
            if (!_defaultParser.TryParse(rawValue, propertyType, out result))
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

      public string ConvertValue(PropertyOptions property, object value)
      {
         string str;

         if (value == null)
         {
            str = null;
         }
         else
         {
            if (_defaultParser.IsSupported(property.Type))
            {
               str = _defaultParser.ToRawString(value);
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
         _parsers.TryGetValue(t, out result);
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
