using System.Reflection;
using Config.Net.Stores;
using System.Collections.Generic;
#if !NETSTANDARD14
using Config.Net.Stores.Impl.CommandLine;
#endif

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
      /// <param name="parseInlineComments">When true, inline comments are parsed. It is set to false by default so inline comments are considered a part of the value.</param>
      /// <returns></returns>
      public static ConfigurationBuilder<TInterface> UseIniFile<TInterface>(this ConfigurationBuilder<TInterface> builder,
         string iniFilePath,
         bool parseInlineComments = false) where TInterface : class
      {
         builder.UseConfigStore(new IniFileConfigStore(iniFilePath, true, parseInlineComments));
         return builder;
      }

      /// <summary>
      /// Simple INI storage.
      /// </summary>
      /// <param name="builder"></param>
      /// <param name="iniString">File contents</param>
      /// <param name="parseInlineComments">When true, inline comments are parsed. It is set to false by default so inline comments are considered a part of the value</param>
      /// <returns></returns>
      public static ConfigurationBuilder<TInterface> UseIniString<TInterface>(this ConfigurationBuilder<TInterface> builder,
         string iniString,
         bool parseInlineComments = false) where TInterface : class
      {
         builder.UseConfigStore(new IniFileConfigStore(iniString, false, parseInlineComments));
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

#if !NETSTANDARD14

      /// <summary>
      /// Accepts builder from the command line arguments. This is not intended to replace a command line parsing framework but rather
      /// complement it in a builder like way. Uses current process' command line parameters automatically
      /// </summary>
      /// <param name="builder">Configuration object</param>
      /// <param name="isCaseSensitive">When true argument names are case sensitive, false by default</param>
      /// <returns>Changed builder</returns>
      public static ConfigurationBuilder<TInterface> UseCommandLineArgs<TInterface>(this ConfigurationBuilder<TInterface> builder,
         bool isCaseSensitive = false,
         params KeyValuePair<string, int>[] parameterNameToPosition)
         where TInterface : class
      {
         builder.UseConfigStore(new CommandLineConfigStore(null, isCaseSensitive, parameterNameToPosition));
         return builder;
      }

      public static ConfigurationBuilder<TInterface> UseCommandLineArgs<TInterface>(this ConfigurationBuilder<TInterface> builder,
         params KeyValuePair<string, int>[] parameterNameToPosition)
         where TInterface : class
      {
         builder.UseConfigStore(new CommandLineConfigStore(null, false, parameterNameToPosition));
         return builder;
      }

#endif

   }
}
