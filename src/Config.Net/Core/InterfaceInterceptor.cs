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
      private readonly bool _isInPc;
      private PropertyChangedEventHandler _inPcHandler;

      public InterfaceInterceptor(Type interfaceType, IoHandler ioHandler, string prefix = null)
      {
         _boxes = BoxFactory.Discover(interfaceType, ioHandler.ValueHandler, prefix);
         _ioHandler = ioHandler;
         _prefix = prefix;
         _reader = new DynamicReader(prefix, ioHandler);
         _writer = new DynamicWriter(prefix, ioHandler);
         _isInPc = interfaceType.GetInterface(nameof(INotifyPropertyChanged)) != null;
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
         if (TryInterceptInPc(invocation)) return;

         ResultBox rBox = FindBox(invocation);

         bool isRead =
            (rBox is PropertyResultBox && PropertyResultBox.IsGetProperty(invocation.Method)) ||
            (rBox is ProxyResultBox && PropertyResultBox.IsGetProperty(invocation.Method)) ||
            (rBox is MethodResultBox mBox && mBox.IsGetter) ||
            (rBox is CollectionResultBox);

         if(isRead)
         {
            invocation.ReturnValue = _reader.Read(rBox, -1, invocation.Arguments);
            return;
         }
         else
         {
            _writer.Write(rBox, invocation.Arguments);

            TryNotifyInPc(invocation, rBox);
         }
      }

      private bool TryInterceptInPc(IInvocation invocation)
      {
         if (!_isInPc) return false;
         
         if (invocation.Method.Name == "add_PropertyChanged")
         {
            invocation.ReturnValue =
               _inPcHandler =
               (PropertyChangedEventHandler)Delegate.Combine(_inPcHandler, (Delegate)invocation.Arguments[0]);
            return true;
         }
         else if(invocation.Method.Name == "remove_PropertyChanged")
         {
            invocation.ReturnValue =
               _inPcHandler =
               (PropertyChangedEventHandler)Delegate.Remove(_inPcHandler, (Delegate)invocation.Arguments[0]);
            return true;
         }

         return false;
      }

      private void TryNotifyInPc(IInvocation invocation, ResultBox rBox)
      {
         if (_inPcHandler == null || rBox is MethodResultBox) return;

         _inPcHandler.Invoke(invocation.InvocationTarget, new PropertyChangedEventArgs(rBox.Name));
         if(rBox.Name != rBox.StoreByName)
         {
            //notify on StoreByName as well
            _inPcHandler.Invoke(invocation.InvocationTarget, new PropertyChangedEventArgs(rBox.StoreByName));
         }
      }
   }
}
