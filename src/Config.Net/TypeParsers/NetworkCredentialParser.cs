using System;
using System.Collections.Generic;
using System.Net;

namespace Config.Net.TypeParsers
{
   class NetworkCredentialParser : ITypeParser
   {
      public IEnumerable<Type> SupportedTypes => new[] { typeof(NetworkCredential) };

      public string ToRawString(object value)
      {
         NetworkCredential nc = value as NetworkCredential;
         return nc?.ToFriendlyString();
      }

      public bool TryParse(string value, Type t, out object result)
      {
         NetworkCredential nc = value.ToNetworkCredential();
         result = nc;
         return true;
      }
   }
}
