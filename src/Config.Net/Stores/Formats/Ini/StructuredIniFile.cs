using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Config.Net.Stores.Formats.Ini
{
   class StructuredIniFile
   {
      private const string SectionBegin = "[";
      private const string SectionEnd = "]";
      private static readonly char[] SectionTrims = {'[', ']'};

      private readonly IniSection _globalSection;
      private readonly List<IniSection> _sections = new List<IniSection>();
      private readonly Dictionary<string, IniKeyValue> _fullKeyNameToValue = new Dictionary<string, IniKeyValue>();

      public StructuredIniFile()
      {
         _globalSection = new IniSection(null);
         _sections.Add(_globalSection);
      }

      public string this[string key]
      {
         get
         {
            if(key == null) return null;

            IniKeyValue value;
            return !_fullKeyNameToValue.TryGetValue(key, out value) ? null : value.Value;
         }
         set
         {
            if(key == null) return;

            string sectionName;
            string keyName;
            IniSection.SplitKey(key, out sectionName, out keyName);
            IniSection section = sectionName == null
               ? _globalSection
               : _sections.FirstOrDefault(s => s.Name == sectionName);
            if(section == null)
            {
               section = new IniSection(sectionName);
               _sections.Add(section);
            }
            IniKeyValue ikv = section.Set(keyName, value);

            //update the local cache
            if(ikv != null)
            {
               if(value == null)
               {
                  _fullKeyNameToValue.Remove(key);
               }
               else
               {
                  _fullKeyNameToValue[key] = ikv;
               }
            }
         }
      }

      public static StructuredIniFile ReadFrom(Stream inputStream)
      {
         if(inputStream == null) throw new ArgumentNullException(nameof(inputStream));

         var file = new StructuredIniFile();

         using(var reader = new StreamReader(inputStream))
         {
            IniSection section = file._globalSection;

            string line;
            while((line = reader.ReadLine()) != null)
            {
               line = line.Trim();

               if(line.StartsWith(SectionBegin))
               {
                  //start new section
                  line = line.Trim(SectionTrims).Trim();
                  section = new IniSection(line);
                  file._sections.Add(section);
               }
               else if(line.StartsWith(IniComment.CommentSeparator))
               {
                  //whole line is a comment
                  string comment = line.Substring(1).Trim();
                  section.Add(new IniComment(comment));
               }
               else
               {
                  IniKeyValue ikv = IniKeyValue.FromLine(line);
                  if(ikv == null) continue;

                  section.Add(ikv);
                  string fullKey = section.Name == null
                     ? ikv.Key
                     : $"{section.Name}{IniSection.SectionKeySeparator}{ikv.Key}";
                  file._fullKeyNameToValue[fullKey] = ikv;

               }
            }
         }

         return file;
      }

      public void WriteTo(Stream outputStream)
      {
         if(outputStream == null) throw new ArgumentNullException(nameof(outputStream));

         using(var writer = new StreamWriter(outputStream))
         {
            foreach(IniSection section in _sections)
            {
               if(section.Name != null)
               {
                  writer.WriteLine();
                  writer.WriteLine($"{SectionBegin}{section.Name}{SectionEnd}");
               }

               section.WriteTo(writer);
            }
         }
      }

      //private static 
   }
}
