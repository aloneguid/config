using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Config.Net.Core.Box
{
   abstract class ResultBox
   {
      private string? _storeByName;

      protected ResultBox(string name, Type resultType, object? defaultResult)
      {
         Name = name;
         ResultType = resultType;
         ResultBaseType = GetBaseType(resultType);
         DefaultResult = defaultResult;
      }

      public string Name { get; }

      public string StoreByName
      {
         get => _storeByName ?? Name;
         set => _storeByName = value;
      }

      public Type ResultType { get; }

      public Type ResultBaseType { get; }

      public object? DefaultResult { get; set; }

      #region [ Utility Methods ]

      private static Type GetBaseType(Type t)
      {
         TypeInfo ti = t.GetTypeInfo();
         if (ti.IsClass)
         {
            return t;
         }
         else
         {
            if (ti.IsGenericType && ti.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
               return ti.GenericTypeArguments[0];
            }
            else
            {
               return t;
            }
         }

      }

      internal static bool TryGetCollection(Type t, out Type elementType)
      {
         TypeInfo ti = t.GetTypeInfo();

         if(ti.IsGenericType && ti.GetGenericTypeDefinition() == typeof(IEnumerable<>))
         {
            elementType = ti.GenericTypeArguments[0];
            return true;
         }

         elementType = t;
         return false;
      }

      #endregion
   }
}
