using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Config.Net.TypeParsers;

namespace Config.Net.Core
{
   class ValueHandler
   {
      private readonly DefaultParser _defaultParser = new DefaultParser();
      private readonly ConcurrentDictionary<Type, ITypeParser> _allParsers = new ConcurrentDictionary<Type, ITypeParser>();
      private readonly HashSet<Type> _supportedTypes = new HashSet<Type>();

      private static readonly ValueHandler _default = new ValueHandler();

      public ValueHandler(IEnumerable<ITypeParser>? customParsers = null)
      {
         foreach (ITypeParser pc in GetBuiltInParsers())
         {
            AddParser(pc);
         }

         if(customParsers != null)
         {
            foreach(ITypeParser pc in customParsers)
            {
               AddParser(pc);
            }
         }
      }

      public void AddParser(ITypeParser parser)
      {
         if (parser == null)
         {
            throw new ArgumentNullException(nameof(parser));
         }

         foreach (Type t in parser.SupportedTypes)
         {
            _allParsers[t] = parser;
            _supportedTypes.Add(t);
         }
      }

      public bool IsSupported(Type t)
      {
         return _supportedTypes.Contains(t) || _defaultParser.IsSupported(t);
      }

      public object? ParseValue(Type baseType, string? rawValue, object? defaultValue)
      {
         object? result;

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

      public bool TryParse(Type propertyType, string? rawValue, out object? result)
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
            ITypeParser? typeParser = GetParser(propertyType);
            if (typeParser == null)
            {
               result = null;
               return false;
            }
            if (!typeParser.TryParse(rawValue, propertyType, out result))
            {
               return false;
            }
         }

         return true;
      }

      public string? ConvertValue(Type baseType, object? value)
      {
         string? str;

         if (value == null)
         {
            str = null;
         }
         else
         {
            if (_defaultParser.IsSupported(baseType))
            {
               str = _defaultParser.ToRawString(value);
            }
            else
            {
               ITypeParser? parser = GetParser(value.GetType());
               str = parser?.ToRawString(value);
            }
         }

         return str;
      }

      private ITypeParser? GetParser(Type t)
      {
         ITypeParser? result;
         _allParsers.TryGetValue(t, out result);
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
            new FloatParser(),
            new DoubleParser(),
            new DecimalParser(),
            new SByteParser(),
            new ShortParser(),
            new IntParser(),
            new LongParser(),
            new ByteParser(),
            new UShortParser(),
            new UIntParser(),
            new ULongParser(),
            new StringArrayParser(),
            new StringParser(),
            new TimeSpanParser(),
            new CoreParsers(),
            new NetworkCredentialParser()
         };
      }

   }
}
