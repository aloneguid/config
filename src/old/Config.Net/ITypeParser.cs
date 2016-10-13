using System;
using System.Collections.Generic;

namespace Config.Net
{
   /// <summary>
   /// Type parser interface
   /// </summary>
   public interface ITypeParser
   {
      /// <summary>
      /// Returns the list of supported types this type parser handles
      /// </summary>
      IEnumerable<Type> SupportedTypes { get; }

      /// <summary>
      /// Tries to parse a value from string
      /// </summary>
      /// <param name="value"></param>
      /// <param name="t"></param>
      /// <param name="result"></param>
      /// <returns></returns>
      bool TryParse(string value, Type t, out object result);

      /// <summary>
      /// Converts value to a string to store in a backed store
      /// </summary>
      /// <param name="value"></param>
      /// <returns></returns>
      string ToRawString(object value);
   }
}
