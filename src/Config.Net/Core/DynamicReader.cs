using System;
using System.Collections.Generic;
using System.Text;
using Config.Net.Core.Box;

namespace Config.Net.Core
{
   class DynamicReader
   {
      private readonly string _basePath;
      private readonly IoHandler _ioHandler;

      public DynamicReader(string basePath, IoHandler ioHandler)
      {
         _basePath = basePath;
         _ioHandler = ioHandler;
      }

      public object Read(ResultBox rbox, int index = -1)
      {
         if(rbox is PropertyResultBox pbox)
         {
            string path = OptionPath.Combine(index, _basePath, pbox.StoreByName);

            return _ioHandler.Read(pbox.ResultBaseType, path, pbox.DefaultResult);
         }

         if(rbox is CollectionResultBox cbox)
         {
            string lengthPath = OptionPath.Combine(index, _basePath, cbox.StoreByName);

            if (!cbox.IsInitialised)
            {
               int length = _ioHandler.GetLength(lengthPath);

               cbox.Initialise(_basePath, length, this);
            }

            return cbox.CollectionInstance;
         }

         throw new NotImplementedException();
      }
   }
}
