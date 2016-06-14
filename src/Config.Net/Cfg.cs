using System;

namespace Config.Net
{
   /// <summary>
   /// Global entry
   /// </summary>
   public static class Cfg
   {
      private static readonly IConfigSource Manager;

      static Cfg()
      {
         Manager = new ConfigManager();
      }

      public static IConfigSource Default => Manager;

      public static IConfigConfiguration Configuration => GlobalConfiguration.Instance;

      /// <summary>
      /// Short syntax for reading from default config manager
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="key"></param>
      /// <returns></returns>
      public static Property<T> Read<T>(Setting<T> key)
      {
         return Manager.Read(key);
      }

      public static Property<T?> Read<T>(Setting<T?> key) where T : struct
      {
         return Manager.Read(key);
      }

      public static void Write<T>(Setting<T> key, T value)
      {
         Manager.Write(key, value);
      }

      public static void Write<T>(Setting<T?> key, T? value) where T : struct
      {
         Manager.Write(key, value);
      }
   }
}
