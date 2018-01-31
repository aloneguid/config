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
         _boxes = BoxFactory.Discover(interfaceType, ioHandler.ValueHandler);
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

         /*if(PropertyResultBox.IsProperty(invocation.Method, out bool isGetProperty, out string propertyName))
         {
            ResultBox rbox = _boxes[propertyName];

            if (rbox is ProxyResultBox proxy)
            {
               if (!proxy.IsInitialised)
               {
                  proxy.Initialise(_ioHandler, OptionPath.Combine(_prefix, proxy.StoreByName));
               }

               if (!isGetProperty)
               {
                  throw new NotSupportedException("cannot assign values to interface properties");
               }

               //return a proxy interface
               invocation.ReturnValue = proxy.ProxyInstance;
            }
            else if(rbox is PropertyResultBox pbox)
            {
               string path = OptionPath.Combine(_prefix, pbox.StoreByName);

               if (isGetProperty)
               {
                  invocation.ReturnValue = _ioHandler.Read(pbox.ResultBaseType, path, pbox.DefaultResult);

               }
               else
               {
                  _ioHandler.Write(pbox.ResultBaseType, path, invocation.Arguments[0]);
               }
            }
            else if(rbox is CollectionResultBox cbox)
            {
               string path = OptionPath.Combine(_prefix, cbox.StoreByName);

               if(!cbox.IsInitialised)
               {
                  int length = _ioHandler.GetLength(path);

                  cbox.Initialise(length);
               }

               invocation.ReturnValue = cbox.CollectionInstance;

               //todo: handle value setters as well
            }
         }
         else
         {
            //it's a method!

            string name = MethodResultBox.GetName(invocation.Method);
            MethodResultBox mbox = (MethodResultBox)_boxes[name];
            string path = mbox.GetValuePath(invocation.Arguments);

            if(mbox.IsGettter)
            {
               invocation.ReturnValue = _ioHandler.Read(mbox.ResultBaseType, path, mbox.DefaultResult);
            }
            else
            {
               object value = invocation.Arguments[invocation.Arguments.Length - 1];
               _ioHandler.Write(mbox.ResultBaseType, path, value);
            }

         }*/
      }

      private string GetPath(MethodInfo mi)
      {
         var parts = new List<string>();

         foreach(ParameterInfo pi in mi.GetParameters())
         {
            OptionAttribute pia = pi.GetCustomAttribute<OptionAttribute>();

            string part = pia?.Alias ?? pi.Name;
         }

         return string.Join(".", parts);
      }
   }
}
