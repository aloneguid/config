using System.Reflection;
using Config.Net.Stores;

namespace Config.Net
{
   /// <summary>
   /// Configuration extensions
   /// </summary>
   public static class ConfigurationExtensions
   {
      /// <summary>
      /// Standard app.config (web.config) configuration store. Read-only.
      /// </summary>
      public static IConfigConfiguration UseAppConfig(this IConfigConfiguration configuration)
      {
         configuration.AddStore(new AppConfigStore());
         return configuration;
      }

      /// <summary>
      /// Reads configuration from the .dll.config or .exe.config file.
      /// </summary>
      /// <param name="configuration"></param>
      /// <param name="assembly">Reference to the assembly to look for</param>
      /// <returns></returns>
      public static IConfigConfiguration UseAssemblyConfig(this IConfigConfiguration configuration, Assembly assembly)
      {
         configuration.AddStore(new AssemblyConfigStore(assembly));
         return configuration;
      }

      /// <summary>
      /// Uses system environment variables
      /// </summary>
      public static IConfigConfiguration UseEnvironmentVariables(this IConfigConfiguration configuration)
      {
         configuration.AddStore(new EnvironmentVariablesStore());
         return configuration;
      }

      /// <summary>
      /// Simple INI storage.
      /// </summary>
      /// <param name="configuration"></param>
      /// <param name="iniFilePath">File does not have to exist, however it will be created as soon as you try to write to it.</param>
      /// <returns></returns>
      public static IConfigConfiguration UseIniFile(this IConfigConfiguration configuration, string iniFilePath)
      {
         configuration.AddStore(new IniFileConfigStore(iniFilePath));
         return configuration;
      }

      /// <summary>
      /// Use in-memory configuration
      /// </summary>
      /// <param name="configuration"></param>
      /// <returns></returns>
      public static IConfigConfiguration UseInMemoryConfig(this IConfigConfiguration configuration)
      {
         configuration.AddStore(new InMemoryConfigStore());
         return configuration;
      }
   }
}
