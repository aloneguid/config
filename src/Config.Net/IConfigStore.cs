using System;

namespace Config.Net
{
   public interface IConfigStore : IDisposable
   {
      string Name { get; }

      bool CanRead { get; }

      bool CanWrite { get; }

      string Read(string key);

      void Write(string key, string value);
   }
}
