using System;
using System.IO;
using Config.Net.Stores.Formats;
using Config.Net.Stores.Formats.Ini;

namespace Config.Net.Stores
{
   /// <summary>
   /// Simple INI storage.
   /// </summary>
   class IniFileConfigStore : IConfigStore
   {
      private readonly string _fullName;
      private readonly string _fileName;
      private StructuredIniFile _iniFile;

      /// <summary>
      /// 
      /// </summary>
      /// <param name="pathName">File does not have to exist, however it will be created as soon as you
      /// try to write to it</param>
      public IniFileConfigStore(string pathName)
      {
         if (pathName == null) throw new ArgumentNullException(nameof(pathName));

         _fullName = Path.GetFullPath(pathName);   // Allow relative path to INI file
         _fileName = Path.GetFileName(_fullName);

         string parentDirPath = Path.GetDirectoryName(_fullName);
         if (string.IsNullOrEmpty(parentDirPath)) throw new IOException("the provided directory path is not valid");
         if (!Directory.Exists(parentDirPath))
         {
            Directory.CreateDirectory(parentDirPath);
         }

         ReadIniFile();
      }

      public string Name => "ini: " + _fileName;

      public bool CanRead => true;

      public bool CanWrite => true;

      public string Read(string fullKey)
      {
         ReadIniFile();

         return _iniFile[fullKey];
      }

      public void Write(string key, string value)
      {
         _iniFile[key] = value;

         WriteIniFile();
      }

      private void ReadIniFile()
      {
         FileInfo iniFile = new FileInfo(_fullName);
         if(iniFile.Exists)
         {
            using(var stream = iniFile.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
               _iniFile = StructuredIniFile.ReadFrom(stream);
            }
         }
         else
         {
            _iniFile = new StructuredIniFile();
         }
      }

      private void WriteIniFile()
      {
         using(var stream = File.Create(_fullName))
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
