using System;
using Config.Net.Core.Box;

namespace Config.Net.Core
{
   class DynamicWriter
   {
      private readonly string _basePath;
      private readonly IoHandler _ioHandler;

      public DynamicWriter(string basePath, IoHandler ioHandler)
      {
         _basePath = basePath;
         _ioHandler = ioHandler;
      }

      public void Write(ResultBox rbox, object[] arguments)
      {
         if (rbox is PropertyResultBox pbox) WriteProperty(pbox, arguments[0]);

         else throw new NotImplementedException();
      }

      private void WriteProperty(PropertyResultBox pbox, object value)
      {
         string path = OptionPath.Combine(_basePath, pbox.StoreByName);

         _ioHandler.Write(pbox.ResultBaseType, path, value);
      }
   }
}
