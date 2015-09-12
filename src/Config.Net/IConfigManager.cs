namespace Config.Net
{
   public interface IConfigManager
   {
      Property<T> Read<T>(Setting<T> key);

      Property<T?> Read<T>(Setting<T?> key) where T : struct;

      void Write<T>(Setting<T> key, T value);

      void Write<T>(Setting<T?> key, T? value) where T : struct;
   }
}
