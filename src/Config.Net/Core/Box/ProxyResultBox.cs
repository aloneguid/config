using System;
using Castle.DynamicProxy;

namespace Config.Net.Core.Box
{
   class ProxyResultBox : ResultBox
   {
      private static readonly ProxyGenerator ProxyGenerator = new ProxyGenerator();

      public ProxyResultBox(string name, Type interfaceType) : base(name, interfaceType, null)
      {
      }

      public bool IsInitialised { get; private set; }

      public object ProxyInstance { get; private set; }

      public void Initialise(IoHandler ioHandler, string prefix)
      {
         ProxyInstance = ProxyGenerator.CreateInterfaceProxyWithoutTarget(ResultBaseType,
            new InterfaceInterceptor(ResultBaseType, ioHandler, prefix));

         IsInitialised = true;
      }
   }
}
