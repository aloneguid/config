using System;
using System.Collections.Generic;
using Castle.DynamicProxy;

namespace Config.Net.Core.Box
{
   class ProxyResultBox : ResultBox
   {
      private static readonly ProxyGenerator ProxyGenerator = new ProxyGenerator();
      private readonly Dictionary<int, object> _indexToProxyInstance = new Dictionary<int, object>();

      public ProxyResultBox(string name, Type interfaceType) : base(name, interfaceType, null)
      {
      }

      public bool IsInitialisedAt(int index)
      {
         return _indexToProxyInstance.ContainsKey(index);
      }

      public object GetInstanceAt(int index)
      {
         return _indexToProxyInstance[index];
      }

      public void InitialiseAt(int index, IoHandler ioHandler, string prefix)
      {
         object instance = ProxyGenerator.CreateInterfaceProxyWithoutTarget(ResultBaseType,
            new InterfaceInterceptor(ResultBaseType, ioHandler, prefix));

         _indexToProxyInstance[index] = instance;
      }
   }
}
