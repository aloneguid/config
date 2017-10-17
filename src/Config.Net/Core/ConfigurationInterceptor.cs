using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Castle.DynamicProxy;

namespace Config.Net.Core
{
   class ConfigurationInterceptor<TInterface> : IInterceptor
   {
      private readonly Dictionary<string, PropertyOptions> _propertyOptions;
      private IoHandler _ioHandler;

      public ConfigurationInterceptor(IoHandler ioHandler)
      {
         _propertyOptions = PropertyOptions.Discover<TInterface>();
         _ioHandler = ioHandler;
      }

      public void Intercept(IInvocation invocation)
      {
         if (IsGetProperty(invocation.Method))
         {
            invocation.ReturnValue = GetValue(invocation.Method);
         }
         else if (IsSetProperty(invocation.Method))
         {
            SetValue(invocation.Method, invocation.Arguments[0]);
         }
         /*else if(IsGetMethod(invocation.Method))
         {
            invocation.ReturnValue = GetMethodValue(invocation.Method);
         }*/
         else
         {
            throw new NotSupportedException("unsupported method signature " + invocation.Method);
         }
      }

      private static bool IsGetProperty(MethodInfo mi)
      {
         return mi.Name.StartsWith("get_");
      }

      private static bool IsSetProperty(MethodInfo mi)
      {
         return mi.Name.StartsWith("set_");
      }

      private static bool IsGetMethod(MethodInfo mi)
      {
         return mi.ReturnType != typeof(void);
      }

      private object GetValue(MethodInfo mi)
      {
         string name = mi.Name.Substring(4);

         PropertyOptions po = _propertyOptions[name];

         return _ioHandler.Read(po);
      }

      private void SetValue(MethodInfo mi, object value)
      {
         string name = mi.Name.Substring(4);

         PropertyOptions po = _propertyOptions[name];

         _ioHandler.Write(po, value);
      }

      private object GetMethodValue(MethodInfo mi)
      {
         string path = GetPath(mi);

         return null;
      }

      private string GetPath(MethodInfo mi)
      {
         var parts = new List<string>();

         foreach(ParameterInfo pi in mi.GetParameters())
         {
            var pia = pi.GetCustomAttribute<OptionAttribute>();

            string part = pia?.Alias ?? pi.Name;            
         }

         return string.Join(".", parts);
      }
   }
}
