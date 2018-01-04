using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Castle.DynamicProxy;
using Config.Net.Core;

namespace Config.Net
{
   public class ConfigurationBuilder<T> where T : class
   {
      private readonly ProxyGenerator _generator = new ProxyGenerator();
      private List<IConfigStore> _stores = new List<IConfigStore>();
      private TimeSpan _cacheInterval = TimeSpan.Zero;

      public ConfigurationBuilder()
      {
         TypeInfo ti = typeof(T).GetTypeInfo();

         if (!ti.IsInterface) throw new ArgumentException($"{ti.FullName} must be an interface", ti.FullName);

         if (!ti.IsVisible) throw new ArgumentException($"{ti.FullName} must be visible outside of the assembly (public)", ti.FullName);
      }

      /// <summary>
      /// Creates an instance of the configuration interface
      /// </summary>
      /// <returns></returns>
      public T Build()
      {
         var valueHandler = new ValueHandler();
         var ioHandler = new IoHandler(_stores, valueHandler, _cacheInterval);

         T instance = _generator.CreateInterfaceProxyWithoutTarget<T>(new ConfigurationInterceptor(typeof(T), ioHandler));

         return instance;
      }

      /// <summary>
      /// Set to anything different from <see cref="TimeSpan.Zero"/> to add caching for values. By default
      /// Config.Net doesn't cache any values
      /// </summary>
      /// <param name="time"></param>
      /// <returns></returns>
      public ConfigurationBuilder<T> CacheFor(TimeSpan time)
      {
         _cacheInterval = time;

         return this;
      }

      public ConfigurationBuilder<T> UseConfigStore(IConfigStore store)
      {
         _stores.Add(store);
         return this;
      }
   }
}
