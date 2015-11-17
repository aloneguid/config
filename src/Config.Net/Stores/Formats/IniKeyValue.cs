using System;

namespace Config.Net.Stores.Formats
{
   class IniKeyValue : IniEntity
   {
      public const string KeyValueSeparator = "=";

      public IniKeyValue(string key, string value, string comment)
      {
         if(key == null) throw new ArgumentNullException(nameof(key));
         Key = key;
         Value = value;
         Comment = comment == null ? null : new IniComment(comment);
      }

      public string Key { get; }

      public string Value { get; set; }

      public IniComment Comment { get; }

      public static IniKeyValue FromLine(string line)
      {
         int idx = line.IndexOf(KeyValueSeparator, StringComparison.InvariantCulture);
         if(idx == -1) return null;

         string key = line.Substring(0, idx).Trim();
         string value = line.Substring(idx + 1).Trim();
         string comment;
         idx = value.IndexOf(IniComment.CommentSeparator, StringComparison.InvariantCulture);
         if(idx != -1)
         {
            comment = value.Substring(idx + 1).Trim();
            value = value.Substring(0, idx).Trim();
         }
         else
         {
            comment = null;
         }
         return new IniKeyValue(key, value, comment);
      }
   }
}
