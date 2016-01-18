namespace Config.Net
{
   /// <summary>
   /// Top interface to read and write configuration values in your application or a part of it.
   /// </summary>
   public interface IConfigSource
   {
      /// <summary>
      /// Reads the value by key
      /// </summary>
      Property<T> Read<T>(Setting<T> key);

      /// <summary>
      /// Reads the nullable value by key
      /// </summary>
      Property<T?> Read<T>(Setting<T?> key) where T : struct;

      /// <summary>
      /// Writes a value by key
      /// </summary>
      void Write<T>(Setting<T> key, T value);

      /// <summary>
      /// Writes a nullable value by key
      /// </summary>
      void Write<T>(Setting<T?> key, T? value) where T : struct;
   }
}
