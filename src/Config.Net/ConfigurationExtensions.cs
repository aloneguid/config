using System.Reflection;
using Config.Net.Stores;
using System.Collections.Generic;

namespace Config.Net
{
   /// <summary>
   /// Configuration extensions
   /// </summary>
   public static class ConfigurationExtensions
   {
#if NETFULL
      /// <summary>
      /// Standard app.config (web.config) builder store. Read-only.
      /// </summary>
      public static ConfigurationBuilder<TInterface> UseAppConfig<TInterface>(this ConfigurationBuilder<TInterface> builder) where TInterface : class
      {
         builder.UseConfigStore(new AppConfigStore());
         return builder;
      }

      /// <summary>
      /// Reads builder from the .dll.config or .exe.config file.
      /// </summary>
      /// <param name="builder"></param>
      /// <param name="assembly">Reference to the assembly to look for</param>
      /// <returns></returns>
      public static ConfigurationBuilder<TInterface> UseAssemblyConfig<TInterface>(this ConfigurationBuilder<TInterface> builder, Assembly assembly) where TInterface : class
      {
         builder.UseConfigStore(new AssemblyConfigStore(assembly));
         return builder;
      }
#endif

      /// <summary>
      /// Uses system environment variables
      /// </summary>
      public static ConfigurationBuilder<TInterface> UseEnvironmentVariables<TInterface>(this ConfigurationBuilder<TInterface> builder) where TInterface : class
      {
         builder.UseConfigStore(new EnvironmentVariablesStore());
         return builder;
      }


      /// <summary>
      /// Simple INI storage.
      /// </summary>
      /// <param name="builder"></param>
      /// <param name="iniFilePath">File does not have to exist, however it will be created as soon as you try to write to it.</param>
      /// <returns></returns>
      public static ConfigurationBuilder<TInterface> UseIniFile<TInterface>(this ConfigurationBuilder<TInterface> builder, string iniFilePath) where TInterface : class
      {
         builder.UseConfigStore(new IniFileConfigStore(iniFilePath));
         return builder;
      }

      /// <summary>
      /// Use in-memory builder
      /// </summary>
      /// <param name="builder"></param>
      /// <returns></returns>
      public static ConfigurationBuilder<TInterface> UseInMemoryConfig<TInterface>(this ConfigurationBuilder<TInterface> builder) where TInterface : class
      {
         builder.UseConfigStore(new InMemoryConfigStore());
         return builder;
      }

      /// <summary>
      /// Accepts builder from the command line arguments. This is not intended to replace a command line parsing framework but rather
      /// complement it in a builder like way. Uses current process' command line parameters automatically
      /// </summary>
      /// <param name="builder">Configuration object</param>
      /// <param name="positionToOption">When parameters are not named you can specify this dictionary to map parameter position to option value.</param>
      /// <returns>Changed builder</returns>
      public static ConfigurationBuilder<TInterface> UseCommandLineArgs<TInterface>(this ConfigurationBuilder<TInterface> builder) where TInterface : class
      {
         builder.UseConfigStore(new CommandLineConfigStore(null));
         return builder;
      }

      /// <summary>
      /// Uses JSON file as a builder storage.
      /// </summary>
      /// <param name="builder">Configuration object.</param>
      /// <param name="jsonFilePath">Full path to json storage file.</param>
      /// <returns>Changed builder.</returns>
      /// <remarks>Storage file does not have to exist, however it will be created as soon as first write performed.</remarks>
      public static ConfigurationBuilder<TInterface> UseJsonFile<TInterface>(this ConfigurationBuilder<TInterface> builder, string jsonFilePath) where TInterface : class
      {
         builder.UseConfigStore(new JsonFileConfigStore(jsonFilePath));
         return builder;
      }

   }
}
