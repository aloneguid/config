using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Config.Net.TypeParsers
{
   class DictionaryParser : ITypeParser
   {
      public IEnumerable<Type> SupportedTypes => new[] { typeof(Dictionary<string, string>), typeof(Dictionary<string, object>) };

      public bool TryParse(string value, Type t, out object result)
      {
         if (value == null)
         {
            result = null;
            return false;
         }

         result = JsonConvert.DeserializeObject(value, t);

         return true;
      }

      public string ToRawString(object value)
      {
         if (value == null) return null;
         return JsonConvert.SerializeObject(value);
      }
   }
}
