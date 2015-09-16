using System;

namespace Config.Net
{
   public static class Cfg
   {
      private static readonly IConfigManager Manager;
      private static readonly IConfigManagerConfig ManagerConfig;

      static Cfg()
      {
         ManagerConfig = new ConfigManagerConfig();
         Manager = new ConfigManager(ManagerConfig);
      }

      public static IConfigManager Default => Manager;

      public static IConfigManagerConfig Configuration => ManagerConfig;

      /// <summary>
      /// Creates a full implementation of <see cref="IConfigManager"/> which contains single store
      /// </summary>
      /// <param name="store"></param>
      /// <returns></returns>
      public static IConfigManager FromStore(IConfigStore store)
      {
         if(store == null) throw new ArgumentNullException(nameof(store));

         var managerConfig = new ConfigManagerConfig();
         managerConfig.AddStore(store);
         return new ConfigManager(managerConfig);
      }

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
