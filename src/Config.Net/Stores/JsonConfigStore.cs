#if CORE3
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Config.Net.Stores
{
   class JsonConfigStore
   {
      public JsonConfigStore()
      {
         JsonDocument doc = JsonDocument.Parse("");
      }
   }
}
#endif