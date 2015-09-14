using System;
using System.Configuration;
using System.IO;
using System.Reflection;

namespace Config.Net.Stores
{
   /// <summary>
   /// Reads configuration from the .dll.config or .exe.config file.
   /// </summary>
   public class AssemblyConfigStore : IConfigStore
   {
      private readonly Configuration _configuration;

      public AssemblyConfigStore(Assembly assembly)
      {
         _configuration = ConfigurationManager.OpenExeConfiguration(assembly.Location);
      }

      public string Name => Path.GetFileName(_configuration.FilePath);

      public bool CanRead => true;

      public bool CanWrite => false;

      public string Read(string key)
      {
         KeyValueConfigurationElement element = _configuration.AppSettings.Settings[key];

         return element?.Value;
      }

      public void Write(string key, string value)
      {
         throw new NotSupportedException();
      }

      public void Dispose()
      {
      }
   }
}
