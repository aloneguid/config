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

      public ConfigurationInterceptor()
      {
         _propertyOptions = PropertyOptions.Discover<TInterface>();
      }

      public void Intercept(IInvocation invocation)
      {
         if(IsGetMethod(invocation.Method))
         {
            invocation.ReturnValue = GetValue(invocation.Method);
         }
      }

      private static bool IsGetMethod(MethodInfo mi)
      {
         return mi.Name.StartsWith("get_");
      }

      private object GetValue(MethodInfo mi)
      {
         string name = mi.Name.Substring(4);

         PropertyOptions po = _propertyOptions[name];

         return po.DefaultValue;
      }
   }
}
