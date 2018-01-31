namespace Config.Net.Core
{
   static class OptionPath
   {
      public const string Separator = ".";

      public static string Combine(params string[] parts)
      {
         return Combine(-1, parts);
      }

      public static string Combine(int index, params string[] parts)
      {
         string s = string.Empty;

         for(int i = 0; i < parts.Length; i++)
         {
            if (s.Length > 0) s += Separator;

            if (!string.IsNullOrEmpty(parts[i])) s += parts[i];
         }

         if(index != -1)
         {
            s = $"{s}[{index}]";
         }

         return s;
      }
   }
}
