using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Config.Net.TypeParsers;

namespace Config.Net
{
   class ContainerConfiguration : IConfigConfiguration
   {
      private readonly ConcurrentBag<IConfigStore> _stores = new ConcurrentBag<IConfigStore>();
      private readonly ConcurrentDictionary<Type, ITypeParser> _parsers = new ConcurrentDictionary<Type, ITypeParser>();

      public TimeSpan CacheTimeout { get; set; }

      public IEnumerable<ITypeParser> Parsers => _parsers.Values;

      public IEnumerable<IConfigStore> Stores => _stores;

      public ContainerConfiguration()
      {
         foreach(ITypeParser pc in GetBuiltInParsers())
         {
            AddParser(pc);
         }
      }

      public void AddStore(IConfigStore store)
      {
         if (store == null) throw new ArgumentNullException(nameof(store));

         _stores.Add(store);
      }

      public void RemoveAllStores()
      {
         IConfigStore store;
         while(!_stores.IsEmpty)
         {
            _stores.TryTake(out store);
         }
      }

      public ITypeParser GetParser(Type t)
      {
         ITypeParser result;
         _parsers.TryGetValue(t, out result);
         return result;
      }

      public bool HasParser(Type t)
      {
         return _parsers.ContainsKey(t);
      }

      public void AddParser(ITypeParser parser)
      {
         if (parser == null) throw new ArgumentNullException(nameof(parser));

         if (parser.SupportedTypes == null) return;

         foreach (Type t in parser.SupportedTypes)
         {
            _parsers[t] = parser;
         }
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
            new DateTimeParser(),
            new CoreParsers(),
            new NetworkCredentialParser()
         };
      }
   }
}
