using System;
using System.IO;
using Config.Net.Stores.Formats;

namespace Config.Net.Stores
{
   /// <summary>
   /// Simple INI storage.
   /// </summary>
   /// <remarks>This implementation DOES NOT handle sections, they will be removed first time you save a value</remarks>
   public class IniFileConfigStore : IConfigStore
   {
      private readonly string _fullName;
      private readonly string _fileName;
      private StructuredIniFile _iniFile;

      //FileSystemWatcher is not perfect and has lots of issues around it, more info : http://weblogs.asp.net/ashben/archive/2003/10/14/31773.aspx
      private readonly FileSystemWatcher _systemWatcher;
      private readonly object _watcherAndAccessLock = new object();

      public event Action ChangesDetected;

      /// <summary>
      /// 
      /// </summary>
      /// <param name="fullName">File does not have to exist, however it will be created as soon as you
      /// try to write to it</param>
      public IniFileConfigStore(string fullName)
      {
         if (fullName == null) throw new ArgumentNullException(nameof(fullName));

         _fileName = Path.GetFileName(fullName);
         _fullName = fullName;

         string parentDirPath = Path.GetDirectoryName(fullName);
         if (string.IsNullOrEmpty(parentDirPath)) throw new IOException("the provided directory path is not valid");
         if (!Directory.Exists(parentDirPath))
         {
            Directory.CreateDirectory(parentDirPath);
         }

         _systemWatcher = new FileSystemWatcher();
         _systemWatcher.Changed += SystemWatcherOnChanged;
         _systemWatcher.Filter = _fileName;
         _systemWatcher.IncludeSubdirectories = false;
         _systemWatcher.NotifyFilter = NotifyFilters.LastWrite;

         ReadIniFile();

         _systemWatcher.Path = parentDirPath; //this requires an existing directory that's why its created above if it does not exist
         _systemWatcher.EnableRaisingEvents = true;
      }

      private void SystemWatcherOnChanged(object sender, FileSystemEventArgs fileSystemEventArgs)
      {
         lock (_watcherAndAccessLock)
         {
            _systemWatcher.EnableRaisingEvents = false;
            try
            {
               ReadIniFile();

               ChangesDetected?.Invoke();
            }
            finally
            {
               //this prevents file watcher from raising multiple events for the same change 
               _systemWatcher.EnableRaisingEvents = true;
            }
         }
      }

      public string Name => "ini: " + _fileName;

      public bool CanRead => true;

      public bool CanWrite => true;

      public string Read(string fullKey)
      {
         //todo : changing to .Net 4.0 and having a ConcurrentDictictionary would increase performance a lot
         lock (_watcherAndAccessLock) //this will make reading slower but we can't read at the same time that someone is changing the file manually and saving it
         {
            return _iniFile[fullKey];
         }
      }

      public void Write(string key, string value)
      {
         lock (_watcherAndAccessLock)
         {
            _iniFile[key] = value;

            //disables filewatcher because we are updating the file in code and not manually no point getting the file changed event
            _systemWatcher.EnableRaisingEvents = false;
            WriteIniFile();
            _systemWatcher.EnableRaisingEvents = true;
         }
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
         if (_systemWatcher != null)
         {
            _systemWatcher.Changed -= SystemWatcherOnChanged;
            _systemWatcher.EnableRaisingEvents = false;
            _systemWatcher.Dispose();
         }
      }
   }
}
