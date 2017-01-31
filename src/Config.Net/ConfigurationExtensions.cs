using System.Reflection;
using Config.Net.Stores;

namespace Config.Net
{
   /// <summary>
   /// Configuration extensions
   /// </summary>
   public static class ConfigurationExtensions
   {
#if NETFULL
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
#endif

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

      /// <summary>
      /// Accepts configuration from the command line arguments. This is not intended to replace a command line parsing framework but rather
      /// complement it in a configuration like way.
      /// </summary>
      /// <param name="configuration">Configuration object</param>
      /// <param name="args">Parameters usually passed to the Main method. This argument is optional and when set to null will get process
      /// command line parameters automatically.</param>
      /// <returns>Chained configuration</returns>
      public static IConfigConfiguration UseCommandLineArgs(this IConfigConfiguration configuration, string[] args)
      {
         configuration.AddStore(new CommandLineConfigStore(args));
         return configuration;
      }

      /// <summary>
      /// Accepts configuration from the command line arguments. This is not intended to replace a command line parsing framework but rather
      /// complement it in a configuration like way. Uses current process' command line parameters automatically
      /// </summary>
      /// <param name="configuration">Configuration object</param>
      /// <returns>Changed configuration</returns>
      public static IConfigConfiguration UseCommandLineArgs(this IConfigConfiguration configuration)
      {
         configuration.AddStore(new CommandLineConfigStore(null));
         return configuration;
      }

   }
}
