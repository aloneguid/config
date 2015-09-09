using System;
using System.Collections.Generic;

namespace Aloneguid.Config
{
   public interface IConfigManagerConfig
   {
      void AddStore(IConfigStore store);

      bool RemoveStore(string name);

      void RemoveAllStores();

      IEnumerable<IConfigStore> Stores { get; }

      IEnumerable<ITypeParser> Parsers { get; }

      bool HasParser(Type t);

      ITypeParser<T> GetParser<T>();

      void RegisterParser<T>(ITypeParser<T> parser);
   }
}
