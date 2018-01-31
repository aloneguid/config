using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Castle.DynamicProxy;
using Config.Net.Core.Box;

namespace Config.Net.Core
{
   class InterfaceInterceptor : IInterceptor
   {
      private readonly Dictionary<string, ResultBox> _boxes;
      private IoHandler _ioHandler;
      private readonly string _prefix;
      private readonly DynamicReader _reader;
      private readonly DynamicWriter _writer;

      public InterfaceInterceptor(Type interfaceType, IoHandler ioHandler, string prefix = null)
      {
         _boxes = BoxFactory.Discover(interfaceType, ioHandler.ValueHandler, prefix);
         _ioHandler = ioHandler;
         _prefix = prefix;
         _reader = new DynamicReader(prefix, ioHandler);
         _writer = new DynamicWriter(prefix, ioHandler);
      }

      private ResultBox FindBox(IInvocation invocation)
      {
         if (PropertyResultBox.IsProperty(invocation.Method, out string propertyName))
         {
            return _boxes[propertyName];
         }
         else //method
         {
            string name = MethodResultBox.GetName(invocation.Method);
            return _boxes[name];
         }
      }

      public void Intercept(IInvocation invocation)
      {
         ResultBox rbox = FindBox(invocation);

         bool isRead =
            (rbox is PropertyResultBox pbox && PropertyResultBox.IsGetProperty(invocation.Method)) ||
            (rbox is ProxyResultBox xbox && PropertyResultBox.IsGetProperty(invocation.Method)) ||
            (rbox is MethodResultBox mbox && mbox.IsGettter) ||
            (rbox is CollectionResultBox);

         if(isRead)
         {
            invocation.ReturnValue = _reader.Read(rbox, -1, invocation.Arguments);
            return;
         }
         else
         {
            _writer.Write(rbox, invocation.Arguments);
         }
      }
   }
}
