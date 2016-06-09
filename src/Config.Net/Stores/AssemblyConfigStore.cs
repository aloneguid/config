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

      /// <summary>
      /// Creates a new instance of assembly configuration store (.dll.config files)
      /// </summary>
      /// <param name="assembly">reference to the assembly to look for</param>
      public AssemblyConfigStore(Assembly assembly)
      {
         _configuration = ConfigurationManager.OpenExeConfiguration(assembly.Location);
      }

      /// <summary>
      /// Store name
      /// </summary>
      public string Name => Path.GetFileName(_configuration.FilePath);

      /// <summary>
      /// Store is readable
      /// </summary>
      public bool CanRead => true;

      /// <summary>
      /// Store is not writeable
      /// </summary>
      public bool CanWrite => false;

      /// <summary>
      /// Reads the value by key
      /// </summary>
      public string Read(string key)
      {
         KeyValueConfigurationElement element = _configuration.AppSettings.Settings[key];

         return element?.Value;
      }

      /// <summary>
      /// Writing is not supported
      /// </summary>
      public void Write(string key, string value)
      {
         throw new NotSupportedException();
      }

      /// <summary>
      /// Nothing to dispose
      /// </summary>
      public void Dispose()
      {
      }
   }
}
