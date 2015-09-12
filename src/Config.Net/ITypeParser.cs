namespace Config.Net
{
   public interface ITypeParser
   {
   }

   public interface ITypeParser<T> : ITypeParser
   {
      bool TryParse(string value, out T result);

      /// <summary>
      /// Converts T to raw string value
      /// </summary>
      /// <param name="value">T value</param>
      /// <returns>Raw string for value of type T</returns>
      string ToRawString(T value);
   }
}
