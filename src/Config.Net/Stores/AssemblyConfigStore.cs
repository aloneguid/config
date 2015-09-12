using System;
using System.Configuration;
using System.IO;
using System.Reflection;

namespace Config.Net.Stores
{
   public class AssemblyConfigStore : IConfigStore
   {
      private readonly Configuration _configuration;

      public AssemblyConfigStore(Assembly assembly)
      {
         _configuration = ConfigurationManager.OpenExeConfiguration(assembly.Location);
      }

      public string Name
      {
         get { return Path.GetFileName(_configuration.FilePath); }
      }

      public bool CanRead
      {
         get { return true; }
      }

      public bool CanWrite
      {
         get { return false; }
      }

      public string Read(string key)
      {
         KeyValueConfigurationElement element = _configuration.AppSettings.Settings[key];

         return element != null ? element.Value : null;
      }

      public void Write(string key, string value)
      {
         throw new NotSupportedException();
      }
   }
}
