using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Config.Net
{
   class Value
   {
      public object RawValue;

      public DateTime Updated;

      public bool IsExpired(TimeSpan ttl)
      {
         return (DateTime.UtcNow - Updated) > ttl;
      }

      public void Update()
      {
         Updated = DateTime.UtcNow;
      }
   }
}
