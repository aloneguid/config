using System;
using System.Collections.Generic;
using System.Text;

namespace Config.Net.Core
{
   public static class OptionPath
   {
      public const string Separator = ".";

      public static string Combine(params string[] parts)
      {
         string s = string.Empty;

         for(int i = 0; i < parts.Length; i++)
         {
            if (s.Length > 0) s += Separator;

            if (!string.IsNullOrEmpty(parts[i])) s += parts[i];
         }

         return s;
      }
   }
}
