using System;
using System.Collections.Generic;
using System.ComponentModel;
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
      private readonly bool _isInpc;
      private PropertyChangedEventHandler _inpcHandler;

      public InterfaceInterceptor(Type interfaceType, IoHandler ioHandler, string prefix = null)
      {
         _boxes = BoxFactory.Discover(interfaceType, ioHandler.ValueHandler, prefix);
         _ioHandler = ioHandler;
         _prefix = prefix;
         _reader = new DynamicReader(prefix, ioHandler);
         _writer = new DynamicWriter(prefix, ioHandler);
         _isInpc = interfaceType.GetInterface(nameof(INotifyPropertyChanged)) != null;
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
         if (TryInterceptInpc(invocation)) return;

         ResultBox rbox = FindBox(invocation);

         bool isRead =
            (rbox is PropertyResultBox && PropertyResultBox.IsGetProperty(invocation.Method)) ||
            (rbox is ProxyResultBox && PropertyResultBox.IsGetProperty(invocation.Method)) ||
            (rbox is MethodResultBox mbox && mbox.IsGetter) ||
            (rbox is CollectionResultBox);

         if(isRead)
         {
            invocation.ReturnValue = _reader.Read(rbox, -1, invocation.Arguments);
            return;
         }
         else
         {
            _writer.Write(rbox, invocation.Arguments);

            TryNotifyInpc(invocation, rbox);
         }
      }

      private bool TryInterceptInpc(IInvocation invocation)
      {
         if (!_isInpc) return false;
         
         if (invocation.Method.Name == "add_PropertyChanged")
         {
            invocation.ReturnValue =
               _inpcHandler =
               (PropertyChangedEventHandler)Delegate.Combine(_inpcHandler, (Delegate)invocation.Arguments[0]);
            return true;
         }
         else if(invocation.Method.Name == "remove_PropertyChanged")
         {
            invocation.ReturnValue =
               _inpcHandler =
               (PropertyChangedEventHandler)Delegate.Remove(_inpcHandler, (Delegate)invocation.Arguments[0]);
            return true;
         }

         return false;
      }

      private void TryNotifyInpc(IInvocation invocation, ResultBox rbox)
      {
         if (_inpcHandler == null || rbox is MethodResultBox) return;

         _inpcHandler.Invoke(invocation.InvocationTarget, new PropertyChangedEventArgs(rbox.Name));
         if(rbox.Name != rbox.StoreByName)
         {
            //notify on StoreByName as well
            _inpcHandler.Invoke(invocation.InvocationTarget, new PropertyChangedEventArgs(rbox.StoreByName));
         }
      }
   }
}
