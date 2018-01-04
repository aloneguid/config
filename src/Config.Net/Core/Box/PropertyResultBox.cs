using System;
using System.Reflection;

namespace Config.Net.Core.Box
{
   class PropertyResultBox : ResultBox
   {
      public PropertyResultBox(string name, Type resultType) : base(name, resultType, null)
      {
      }

      public static bool IsProperty(MethodInfo mi, out bool isGetter, out string name)
      {
         if (mi.Name.StartsWith("get_"))
         {
            isGetter = true;
            name = mi.Name.Substring(4);
            return true;
         }

         if (mi.Name.StartsWith("set_"))
         {
            isGetter = false;
            name = mi.Name.Substring(4);
            return true;
         }

         isGetter = false;
         name = null;
         return false;
      }

   }
}
