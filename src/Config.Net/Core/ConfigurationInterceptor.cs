using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Castle.DynamicProxy;
using Config.Net.Core.Box;

namespace Config.Net.Core
{
   class ConfigurationInterceptor : IInterceptor
   {
      private readonly Dictionary<string, ResultBox> _boxes;
      private readonly Type _interfaceType;
      private IoHandler _ioHandler;
      private readonly string _prefix;

      public ConfigurationInterceptor(Type interfaceType, IoHandler ioHandler, string prefix = null)
      {
         _boxes = BoxFactory.Discover(interfaceType, ioHandler.ValueHandler);
         _interfaceType = interfaceType;
         _ioHandler = ioHandler;
         _prefix = prefix;
      }

      public void Intercept(IInvocation invocation)
      {
         if(PropertyResultBox.IsProperty(invocation.Method, out bool isGetProperty, out string propertyName))
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
            else
            {
               PropertyResultBox pbox = (PropertyResultBox)rbox;
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

         }
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
