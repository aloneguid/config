namespace Config.Net
{
   /// <summary>
   /// Type parser interface
   /// </summary>
   public interface ITypeParser
   {
   }

   /// <summary>
   /// Type parser interface
   /// </summary>
   /// <typeparam name="T"></typeparam>
   public interface ITypeParser<T> : ITypeParser
   {
      /// <summary>
      /// Tries to parse out the type from string representation
      /// </summary>
      bool TryParse(string value, out T result);

      /// <summary>
      /// Converts T to raw string value
      /// </summary>
      /// <param name="value">T value</param>
      /// <returns>Raw string for value of type T</returns>
      string ToRawString(T value);
   }
}
