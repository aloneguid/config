using System;

namespace Config.Net
{
   /// <summary>
   /// Type parser interface
   /// </summary>
   public interface ITypeParser
   {
      bool TryParse(string value, Type t, out object result);
      string ToRawString(object value);
   }
}
