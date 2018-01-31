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

      public object Read(ResultBox rbox, int index = -1, params object[] arguments)
      {
         if (rbox is PropertyResultBox pbox) return ReadProperty(pbox, index);

         if (rbox is ProxyResultBox xbox) return ReadProxy(xbox, index);

         if (rbox is CollectionResultBox cbox) return ReadCollection(cbox, index);

         if (rbox is MethodResultBox mbox) return ReadMethod(mbox, arguments);

         throw new NotImplementedException($"don't know how to read {rbox.GetType()}");
      }

      private object ReadProperty(PropertyResultBox pbox, int index)
      {
         string path = OptionPath.Combine(index, _basePath, pbox.StoreByName);

         return _ioHandler.Read(pbox.ResultBaseType, path, pbox.DefaultResult);
      }

      private object ReadProxy(ProxyResultBox xbox, int index)
      {
         if (!xbox.IsInitialisedAt(index))
         {
            string prefix = OptionPath.Combine(index, _basePath, xbox.StoreByName);

            xbox.InitialiseAt(index, _ioHandler, prefix);
         }

         return xbox.GetInstanceAt(index);
      }

      private object ReadCollection(CollectionResultBox cbox, int index)
      {
         string lengthPath = OptionPath.Combine(index, _basePath, cbox.StoreByName);

         if (!cbox.IsInitialised)
         {
            int length = _ioHandler.GetLength(lengthPath);

            cbox.Initialise(_basePath, length, this);
         }

         return cbox.CollectionInstance;
      }

      private object ReadMethod(MethodResultBox mbox, object[] arguments)
      {
         string path = mbox.GetValuePath(arguments);

         return _ioHandler.Read(mbox.ResultBaseType, path, mbox.DefaultResult);
      }
   }
}
