using System;
using System.Collections.Generic;
using System.Text;
using Castle.DynamicProxy;
using Config.Net.Core;

namespace Config.Net
{
   public class ConfigurationBuilder<T> where T : class
   {
      private readonly ProxyGenerator _generator = new ProxyGenerator();

      public T Create()
      {
         T instance = _generator.CreateInterfaceProxyWithoutTarget<T>(new ConfigurationInterceptor<T>());

         return instance;
      }
   }
}
