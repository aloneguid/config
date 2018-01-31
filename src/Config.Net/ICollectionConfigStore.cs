using System;
using System.Collections.Generic;
using System.Text;

namespace Config.Net
{
   public interface ICollectionConfigStore : IConfigStore
   {
      int GetCollectionLength(string key);
   }
}
