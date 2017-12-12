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
         if(IsProperty(invocation.Method, out bool isGetProperty, out string propertyName))
         {
            PropertyOptions po = _propertyOptions[propertyName];

            if (isGetProperty)
            {
               invocation.ReturnValue = _ioHandler.Read(po);
            }
            else
            {
               _ioHandler.Write(po, invocation.Arguments[0]);
            }
         }
         else
         {
            //it's a method!

            string name = PropertyOptions.GetMethodName(invocation.Method);
            PropertyOptions po = _propertyOptions[name];

            if(po.IsGetter == null)
            {
               throw new ArgumentException($"it seems like we don't know whether method '{name}' is a getter or a setter!");
            }

            bool isGetter = po.IsGetter.Value;
            string keyName = GetValuePath(po, invocation.Arguments, !isGetter);
            if(isGetter)
            {
               invocation.ReturnValue = _ioHandler.Read(po, keyName);
            }
            else
            {
               _ioHandler.Write(po, invocation.Arguments[invocation.Arguments.Length - 1], keyName);
            }

         }
      }

      private static bool IsProperty(MethodInfo mi, out bool isGetter, out string name)
      {
         if(mi.Name.StartsWith("get_"))
         {
            isGetter = true;
            name = mi.Name.Substring(4);
            return true;
         }

         if(mi.Name.StartsWith("set_"))
         {
            isGetter = false;
            name = mi.Name.Substring(4);
            return true;
         }

         isGetter = false;
         name = null;
         return false;
      }

      private static string GetValuePath(PropertyOptions po, object[] arguments, bool ignoreLast)
      {
         var sb = new StringBuilder();
         sb.Append(po.StoreName);

         for(int i = 0; i < arguments.Length - (ignoreLast ? 1 : 0); i++)
         {
            object value = arguments[i];
            if (value == null) continue;

            sb.Append(PropertyOptions.PathSeparator);
            sb.Append(value.ToString());
         }

         return sb.ToString();
      }

      // -----
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
            OptionAttribute pia = pi.GetCustomAttribute<OptionAttribute>();

            string part = pia?.Alias ?? pi.Name;
         }

         return string.Join(".", parts);
      }
   }
}
