using System;

namespace Config.Net
{
   /// <summary>
   /// Global entry
   /// </summary>
   public static class Cfg
   {
      private static readonly ConfigManager Manager = new ConfigManager();

      /// <summary>
      /// Configuration endpoint
      /// </summary>
      public static IConfigConfiguration Configuration => GlobalConfiguration.Instance;

      /// <summary>
      /// Reads property from configuration
      /// </summary>
      /// <typeparam name="T">Property type</typeparam>
      /// <param name="key">Property definition</param>
      /// <returns></returns>
      public static Property<T> Read<T>(Setting<T> key)
      {
         return Manager.Read(key);
      }

      /// <summary>
      /// Reads property from configuration
      /// </summary>
      /// <typeparam name="T">Property type</typeparam>
      /// <param name="key">Property definition</param>
      /// <returns></returns>
      public static Property<T?> Read<T>(Setting<T?> key) where T : struct
      {
         return Manager.Read(key);
      }

      /// <summary>
      /// Writes setting value
      /// </summary>
      /// <typeparam name="T">Value type</typeparam>
      /// <param name="key">Settings definition</param>
      /// <param name="value">New value</param>
      public static void Write<T>(Setting<T> key, T value)
      {
         Manager.Write(key, value);
      }

      /// <summary>
      /// Writes setting value
      /// </summary>
      /// <typeparam name="T">Value type</typeparam>
      /// <param name="key">Settings definition</param>
      /// <param name="value">New value</param>
      public static void Write<T>(Setting<T?> key, T? value) where T : struct
      {
         Manager.Write(key, value);
      }
   }
}
