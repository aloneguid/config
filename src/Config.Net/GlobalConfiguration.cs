using System;
using System.Collections.Generic;
using System.Linq;
using Config.Net.TypeParsers;

namespace Config.Net
{
   class GlobalConfiguration : IConfigConfiguration
   {
      private readonly List<IConfigStore> _stores = new List<IConfigStore>();
      private readonly Dictionary<Type, ITypeParser> TypeParsers = GetBuiltInParsers();
      private readonly object _lock = new object();

      private static GlobalConfiguration _instance;

      public static GlobalConfiguration Instance
      {
         get
         {
            if(_instance == null) _instance = new GlobalConfiguration();

            return _instance;
         }
      }

      private GlobalConfiguration()
      {
         DefaultParser = new DefaultParser();
      }

      public DefaultParser DefaultParser
      {
         get;
      }

      public bool CanParse(Type t)
      {
         return HasParser(t) || DefaultParser.IsSupported(t);
      }

      public void AddStore(IConfigStore store)
      {
         if(store == null) throw new ArgumentNullException(nameof(store));

         lock(_lock)
         {
            if(_stores.Any(s => s.Name == store.Name))
            {
               return;
            }

            _stores.Add(store);
         }
      }

      public bool RemoveStore(string name)
      {
         if(string.IsNullOrEmpty(name)) return false;

         lock(_lock)
         {
            IConfigStore store = null;
            foreach(IConfigStore s in _stores)
            {
               if(s.Name == name)
               {
                  store = s;
                  break;
               }
            }

            if(store == null) return false;

            _stores.Remove(store);
            return true;
         }
      }

      public void RemoveAllStores()
      {
         lock(_lock)
         {
            _stores.Clear();
         }
      }

      public IEnumerable<IConfigStore> Stores
      {
         get
         {
            lock(_lock)
            {
               return new List<IConfigStore>(_stores);
            }
         }
      }

      public IEnumerable<ITypeParser> Parsers
      {
         get
         {
            lock(_lock)
            {
               return new List<ITypeParser>(TypeParsers.Values);
            }
         }
      }

      public bool HasParser(Type t)
      {
         Type nullable = Nullable.GetUnderlyingType(t);
         lock(_lock)
         {
            return nullable != null ? TypeParsers.ContainsKey(nullable) : TypeParsers.ContainsKey(t);
         }
      }

      public ITypeParser<T> GetParser<T>()
      {
         lock(_lock)
         {
            return TypeParsers.ContainsKey(typeof(T)) ? (ITypeParser<T>) TypeParsers[typeof(T)] : null;
         }
      }

      private void RegisterParser<T>(ITypeParser<T> parser)
      {
         if(parser == null) throw new ArgumentNullException("parser");
         lock(_lock)
         {
            TypeParsers[typeof(T)] = parser;
         }
      }

      /// <summary>
      /// Scans assembly for types implementing <see cref="ITypeParser"/> and builds Type => instance dictionary.
      /// Not sure if I should use reflection here, however the assembly is small and this shouldn't cause any
      /// performance issues
      /// </summary>
      /// <returns></returns>
      private static Dictionary<Type, ITypeParser> GetBuiltInParsers()
      {
         //initialise dictionary manually intead of reflection for performance
         return new Dictionary<Type, ITypeParser>
         {
            {typeof(bool), new BoolParser()},
            {typeof(double), new DoubleParser()},
            {typeof(int), new IntParser()},
            {typeof(JiraTime), new JiraTimeParser()},
            {typeof(long), new LongParser()},
            {typeof(string[]), new StringArrayParser()},
            {typeof(string), new StringParser()},
            {typeof(TimeSpan), new TimeSpanParser()},
            {typeof(DateTime), new DateTimeParser()}
         };
      }

   }
}
