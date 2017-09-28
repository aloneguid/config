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
      private List<IConfigStore> _stores = new List<IConfigStore>();

      public T Build()
      {
         var handler = new IoHandler(_stores);

         T instance = _generator.CreateInterfaceProxyWithoutTarget<T>(new ConfigurationInterceptor<T>(handler));

         return instance;
      }

      public ConfigurationBuilder<T> UseConfigStore(IConfigStore store)
      {
         _stores.Add(store);
         return this;
      }
   }
}
