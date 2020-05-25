using System;
using System.IO;
using System.Text;
using Config.Net.Core;
using Config.Net.Stores.Formats.Ini;

namespace Config.Net.Stores
{
   /// <summary>
   /// Simple INI storage.
   /// </summary>
   class IniFileConfigStore : IConfigStore
   {
      private readonly string _fullName;
      private readonly StructuredIniFile _iniFile;

      /// <summary>
      /// 
      /// </summary>r
      /// <param name="name">File does not have to exist, however it will be created as soon as you
      /// try to write to it</param>
      public IniFileConfigStore(string name, bool isFilePath, bool parseInlineComments)
      {
         if (name == null) throw new ArgumentNullException(nameof(name));

         if (isFilePath)
         {
            _fullName = Path.GetFullPath(name);   // Allow relative path to INI file

            string parentDirPath = Path.GetDirectoryName(_fullName);
            if (string.IsNullOrEmpty(parentDirPath)) throw new IOException("the provided directory path is not valid");
            if (!Directory.Exists(parentDirPath))
            {
               Directory.CreateDirectory(parentDirPath);
            }

            _iniFile = ReadIniFile(_fullName, parseInlineComments);

            CanWrite = true;
         }
         else
         {
            _iniFile = ReadIniContent(name, parseInlineComments);

            CanWrite = false;
         }

         CanRead = true;
      }

      public string Name => ".ini";

      public bool CanRead { get; }

      public bool CanWrite { get; }

      public string Read(string key)
      {
         if (FlatArrays.IsArrayLength(key, k => _iniFile[k], out int length))
         {
            return length.ToString();
         }

         if (FlatArrays.IsArrayElement(key, k => _iniFile[k], out string element))
         {
            return element;
         }

         return _iniFile[key];
      }

      public void Write(string key, string value)
      {
         if (!CanWrite) return;

         _iniFile[key] = value;

         WriteIniFile();
      }

      private static StructuredIniFile ReadIniFile(string fullName, bool parseInlineComments)
      {
         FileInfo iniFile = new FileInfo(fullName);
         if(iniFile.Exists)
         {
            using(FileStream stream = iniFile.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
               return StructuredIniFile.ReadFrom(stream, parseInlineComments);
            }
         }
         else
         {
            return new StructuredIniFile();
         }
      }

      private static StructuredIniFile ReadIniContent(string content, bool parseInlineComments)
      {
         using (Stream input = new MemoryStream(Encoding.UTF8.GetBytes(content)))
         {
            return StructuredIniFile.ReadFrom(input, parseInlineComments);
         }
      }

      private void WriteIniFile()
      {
         using(FileStream stream = File.Create(_fullName))
         {
            _iniFile.WriteTo(stream);
         }
      }

      public void Dispose()
      {
         //nothing to dispose
      }
   }
}
