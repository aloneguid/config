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

      public object Read(ResultBox rBox, int index = -1, params object[] arguments)
      {
         if (rBox is PropertyResultBox pBox) return ReadProperty(pBox, index);

         if (rBox is ProxyResultBox xBox) return ReadProxy(xBox, index);

         if (rBox is CollectionResultBox cBox) return ReadCollection(cBox, index);

         if (rBox is MethodResultBox mBox) return ReadMethod(mBox, arguments);

         throw new NotImplementedException($"don't know how to read {rBox.GetType()}");
      }

      private object ReadProperty(PropertyResultBox pBox, int index)
      {
         string path = OptionPath.Combine(index, _basePath, pBox.StoreByName);

         return _ioHandler.Read(pBox.ResultBaseType, path, pBox.DefaultResult);
      }

      private object ReadProxy(ProxyResultBox xBox, int index)
      {
         if (!xBox.IsInitialisedAt(index))
         {
            string prefix = OptionPath.Combine(index, _basePath, xBox.StoreByName);

            xBox.InitialiseAt(index, _ioHandler, prefix);
         }

         return xBox.GetInstanceAt(index);
      }

      private object ReadCollection(CollectionResultBox cBox, int index)
      {
         string lengthPath = OptionPath.Combine(index, _basePath, cBox.StoreByName);
         lengthPath = OptionPath.AddLength(lengthPath);

         if (!cBox.IsInitialised)
         {
            int length = (int)_ioHandler.Read(typeof(int), lengthPath, 0);

            cBox.Initialise(_basePath, length, this);
         }

         return cBox.CollectionInstance;
      }

      private object ReadMethod(MethodResultBox mBox, object[] arguments)
      {
         string path = mBox.GetValuePath(arguments);
         path = OptionPath.Combine(_basePath, path);

         return _ioHandler.Read(mBox.ResultBaseType, path, mBox.DefaultResult);
      }
   }
}
