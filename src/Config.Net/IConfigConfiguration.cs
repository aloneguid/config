using System;
using System.Collections.Generic;

namespace Config.Net
{
   /// <summary>
   /// Entry point to configuration options
   /// </summary>
   public interface IConfigConfiguration
   {
      /// <summary>
      /// Adds configuration store
      /// </summary>
      void AddStore(IConfigStore store);

      /// <summary>
      /// Removes all cofnigures stores
      /// </summary>
      void RemoveAllStores();

      /// <summary>
      /// Returns list of registered configuration stores
      /// </summary>
      IEnumerable<IConfigStore> Stores { get; }

      /// <summary>
      /// Returns list of registered parsers
      /// </summary>
      IEnumerable<ITypeParser> Parsers { get; }

      /// <summary>
      /// Returns true if type can be parsed
      /// </summary>
      bool HasParser(Type t);

      /// <summary>
      /// Gets parser implementaton by type, or returns null if there is no registered implementation
      /// </summary>
      ITypeParser GetParser(Type t);

      /// <summary>
      /// Timeout when property cache will expire and the library will try to read it from the backend again.
      /// Setting this value to <see cref="TimeSpan.Zero"/> will cause the library not to cache at all.
      /// </summary>
      TimeSpan CacheTimeout { get; set; }
   }
}
