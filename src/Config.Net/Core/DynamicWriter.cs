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

      public void Write(ResultBox rBox, object[] arguments)
      {
         if (rBox is PropertyResultBox pBox) WriteProperty(pBox, arguments);

         else if (rBox is MethodResultBox mBox) WriteMethod(mBox, arguments);

         else if (rBox is ProxyResultBox xBox) WriteProxy(xBox, arguments);

         else throw new NotImplementedException($"don't know how to write {rBox.GetType()}");
      }

      private void WriteProperty(PropertyResultBox pBox, object[] arguments)
      {
         string path = OptionPath.Combine(_basePath, pBox.StoreByName);

         _ioHandler.Write(pBox.ResultBaseType, path, arguments[0]);
      }

      private void WriteMethod(MethodResultBox mBox, object[] arguments)
      {
         object value = arguments[arguments.Length - 1];
         string path = mBox.GetValuePath(arguments);

         _ioHandler.Write(mBox.ResultBaseType, path, value);
      }

      private void WriteProxy(ProxyResultBox xBox, object[] arguments)
      {
         throw new NotSupportedException("cannot assign values to interface properties");
      }
   }
}
