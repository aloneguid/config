using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Config.Net.Core.Box
{
   class MethodResultBox : ResultBox
   {
      public MethodResultBox(MethodInfo methodInfo) : base(GetName(methodInfo), GetReturnType(methodInfo), null)
      {
         StoreByName = GetStoreName(methodInfo);
         IsGettter = IsGet(methodInfo);
      }

      public bool IsGettter { get; private set; }

      /// <summary>
      /// Composes a uniqueue method name using method name itself and parameter type names, separated by underscore
      /// </summary>
      public static string GetName(MethodInfo mi)
      {
         ParameterInfo[] parameters = mi.GetParameters();
         var sb = new StringBuilder();
         sb.Append(mi.Name);
         foreach (ParameterInfo pi in parameters)
         {
            sb.Append("-");
            sb.Append(pi.ParameterType.ToString());
         }
         return sb.ToString();
      }

      public string GetValuePath(object[] arguments)
      {
         var sb = new StringBuilder();
         sb.Append(StoreByName);
         bool ignoreLast = !IsGettter;

         for (int i = 0; i < arguments.Length - (ignoreLast ? 1 : 0); i++)
         {
            object value = arguments[i];
            if (value == null) continue;

            if (sb.Length > 0)
            {
               sb.Append(OptionPath.Separator);
            }
            sb.Append(value.ToString());
         }

         return sb.ToString();
      }

      private static string GetStoreName(MethodInfo mi)
      {
         string name = mi.Name;

         if (name.StartsWith("get", StringComparison.OrdinalIgnoreCase) ||
            name.StartsWith("set", StringComparison.OrdinalIgnoreCase))
         {
            name = name.Substring(3);
         }

         return name;
      }

      private static bool IsGet(MethodInfo mi)
      {
         return mi.ReturnType != typeof(void);
      }

      private static Type GetReturnType(MethodInfo mi)
      {
         ParameterInfo[] parameters = mi.GetParameters();

         if (parameters == null || parameters.Length == 0)
         {
            throw new InvalidOperationException($"method {mi.Name} must have at least one parameter");
         }

         Type returnType = IsGet(mi) ? mi.ReturnType : parameters[parameters.Length - 1].ParameterType;

         return returnType;
      }
   }
}
