namespace Config.Net.Core
{
   public static class OptionPath
   {
      public const string Separator = ".";
      private const string IndexOpen = "[";
      private const string IndexClose = "]";
      public const string LengthFunction = ".$l";

      public static string Combine(params string[] parts)
      {
         return Combine(-1, parts);
      }

      public static string AddLength(string path)
      {
         return path + LengthFunction;
      }

      public static bool TryStripLength(string path, out string noLengthPath)
      {
         if(!path.EndsWith(LengthFunction))
         {
            noLengthPath = path;
            return false;
         }

         noLengthPath = path.Substring(0, path.Length - LengthFunction.Length);
         return true;
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
            s = $"{s}{IndexOpen}{index}{IndexClose}";
         }

         return s;
      }
   }
}
