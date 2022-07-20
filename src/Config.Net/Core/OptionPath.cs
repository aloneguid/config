namespace Config.Net.Core
{
   public static class OptionPath
   {
      public const string Separator = ".";
      private const string IndexOpen = "[";
      private const string IndexClose = "]";
      public const string LengthFunction = ".$l";

      public static string Combine(params string?[] parts)
      {
         return Combine(-1, parts);
      }

      public static string AddLength(string path)
      {
         return path + LengthFunction;
      }

      public static bool TryStripLength(string? path, out string? noLengthPath)
      {
         if (path == null)
         {
            noLengthPath = path;
            return false;
         }

         if (!path.EndsWith(LengthFunction))
         {
            noLengthPath = path;
            return false;
         }

         noLengthPath = path.Substring(0, path.Length - LengthFunction.Length);
         return true;
      }

      /// <summary>
      /// For indexed paths like "creds[1]" strips index part so it becomes:
      /// - noIndexPath: "creds"
      /// - index: 1
      /// 
      /// If path is not indexed returns false and noIndexPath is equal to path itself
      /// </summary>
      public static bool TryStripIndex(string? path, out string? noIndexPath, out int index)
      {
         if (path == null)
         {
            index = 0;
            noIndexPath = path;
            return false;
         }

         int openIdx = path.IndexOf(IndexOpen);
         int closeIdx = path.IndexOf(IndexClose);

         if (openIdx == -1 || closeIdx == -1 || openIdx > closeIdx || closeIdx != path.Length - 1)
         {
            noIndexPath = path;
            index = 0;
            return false;
         }

         noIndexPath = path.Substring(0, openIdx);
         int.TryParse(path.Substring(openIdx + 1, closeIdx - openIdx - 1), out index);
         return true;
      }

      public static string Combine(int index, params string?[] parts)
      {
         string s = string.Empty;

         for (int i = 0; i < parts.Length; i++)
         {
            if (s.Length > 0) s += Separator;

            if (!string.IsNullOrEmpty(parts[i])) s += parts[i];
         }

         if (index != -1)
         {
            s = $"{s}{IndexOpen}{index}{IndexClose}";
         }

         return s;
      }
   }
}