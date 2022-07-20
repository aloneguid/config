using System;
using System.Collections.Generic;
using System.IO;

namespace Config.Net.Stores.Formats.Ini
{
   class IniSection
   {
      public const string SectionKeySeparator = ".";

      private readonly List<IniEntity> _entities = new List<IniEntity>();
      private readonly Dictionary<string, IniKeyValue> _keyToValue = new Dictionary<string, IniKeyValue>();

      /// <summary>
      /// Section name
      /// </summary>
      public string? Name { get; set; }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="name">Pass null to work with global section</param>
      public IniSection(string? name)
      {
         if(name != null)
         {
            if (name.StartsWith("[")) name = name.Substring(1);
            if (name.EndsWith("]")) name = name.Substring(0, name.Length - 1);
         }

         Name = name;
      }

      public void Add(IniEntity entity)
      {
         _entities.Add(entity);

         IniKeyValue? ikv = entity as IniKeyValue;
         if(ikv != null)
         {
            _keyToValue[ikv.Key] = ikv;
         }
      }

      public IniKeyValue? Set(string key, string? value)
      {
         if(value == null)
         {
            IniKeyValue? ikv;
            if(_keyToValue.TryGetValue(key, out ikv))
            {
               _keyToValue.Remove(key);
               return ikv;
            }
            return null;
         }
         else
         {
            IniKeyValue? ikv;
            if(_keyToValue.TryGetValue(key, out ikv))
            {
               ikv.Value = value;
            }
            else
            {
               ikv = new IniKeyValue(key, value, null);
               Add(ikv);
            }
            return ikv;
         }
      }

      public static void SplitKey(string fullKey, out string? sectionName, out string keyName)
      {
         int idx = fullKey.IndexOf(SectionKeySeparator, StringComparison.CurrentCulture);

         if(idx == -1)
         {
            sectionName = null;
            keyName = fullKey;
         }
         else
         {
            sectionName = fullKey.Substring(0, idx);
            keyName = fullKey.Substring(idx + 1);
         }
      }

      public void WriteTo(StreamWriter writer)
      {
         foreach(IniEntity entity in _entities)
         {
            IniKeyValue? ikv = entity as IniKeyValue;
            if(ikv != null)
            {
               writer.Write($"{ikv.Key}{IniKeyValue.KeyValueSeparator}{ikv.Value}");
               if(ikv.Comment != null)
               {
                  writer.Write(" ");
                  writer.Write(IniComment.CommentSeparator);
                  writer.Write(ikv.Comment.Value);
               }
               writer.WriteLine();
               continue;
            }

            IniComment? comment = entity as IniComment;
            if(comment != null)
            {
               writer.Write(IniComment.CommentSeparator);
               writer.WriteLine(comment.Value);
            }
         }
      }

      public override string ToString()
      {
         return Name ?? string.Empty;
      }
   }
}
