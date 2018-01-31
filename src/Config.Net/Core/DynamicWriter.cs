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
         if (rbox is PropertyResultBox pbox) WriteProperty(pbox, arguments);

         else if (rbox is MethodResultBox mbox) WriteMethod(mbox, arguments);

         else if (rbox is ProxyResultBox xbox) WriteProxy(xbox, arguments);

         else throw new NotImplementedException($"don't know how to write {rbox.GetType()}");
      }

      private void WriteProperty(PropertyResultBox pbox, object[] arguments)
      {
         string path = OptionPath.Combine(_basePath, pbox.StoreByName);

         _ioHandler.Write(pbox.ResultBaseType, path, arguments[0]);
      }

      private void WriteMethod(MethodResultBox mbox, object[] arguments)
      {
         object value = arguments[arguments.Length - 1];
         string path = mbox.GetValuePath(arguments);

         _ioHandler.Write(mbox.ResultBaseType, path, value);
      }

      private void WriteProxy(ProxyResultBox xbox, object[] arguments)
      {
         throw new NotSupportedException("cannot assign values to interface properties");
      }
   }
}
