using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Aloneguid.Config.Stores
{
   /// <summary>
   /// Simple INI storage.
   /// </summary>
   /// <remarks>This implementation DOES NOT handle sections, they will be removed first time you save a value</remarks>
   public class IniFileConfigStore : IConfigStore, IDisposable
   {
      private readonly string _fullName;
      private readonly string _fileName;
      private readonly Dictionary<string, IniValue> _keyToValue = new Dictionary<string, IniValue>();

      //FileSystemWatcher is not perfect and has lots of issues around it, more info : http://weblogs.asp.net/ashben/archive/2003/10/14/31773.aspx
      private readonly FileSystemWatcher _systemWatcher;
      private readonly object _watcherAndAccessLock = new object();

      private class IniValue
      {
         public IniValue(string value)
         {
            int idx = -1;
            if (value != null) idx = value.IndexOf(';');
            if (idx == -1)
            {
               Value = value;
            }
            else
            {
               if (value != null)
               {
                  Value = value.Substring(0, idx).Trim();
                  Comment = value.Substring(idx + 1).Trim();
               }
            }
         }

         public string Value { get; set; }

         // ReSharper disable once MemberCanBePrivate.Local
         public string Comment { get; set; }

         public override string ToString()
         {
            if (string.IsNullOrEmpty(Comment)) return Value;

            return string.Format("{0}; {1}", Value, Comment);
         }
      }

      private class KeyValue
      {
         readonly char[] _separator = { '=' };
         public KeyValue(string line)
         {
            string[] keyValue = line.Split(_separator, 2, StringSplitOptions.RemoveEmptyEntries);
            if (keyValue.Length == 2)
            {
               Key = keyValue[0].Trim();
               Value = keyValue[1].Trim();
            }
            else
            {
               Key = null;
               Value = null;
            }
         }

         public string Key { get; private set; }

         public string Value { get; private set; }
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="fullName">File does not have to exist, however it will be created as soon as you
      /// try to write to it</param>
      public IniFileConfigStore(string fullName)
      {
         if (fullName == null) throw new ArgumentNullException("fullName");

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
            ReadIniFile();
            //this prevents file watcher from raising multiple events for the same change 
            _systemWatcher.EnableRaisingEvents = true;
         }
      }

      public string Name
      {
         get { return "ini:" + _fileName; }
      }

      public bool CanRead { get { return true; } }
      public bool CanWrite { get { return true; } }

      public string Read(string fullKey)
      {
         //todo : changing to .Net 4.0 and having a ConcurrentDictictionary would increase performance a lot
         lock (_watcherAndAccessLock) //this will make reading slower but we can't read at the same time that someone is changing the file manually and saving it
         {
            return _keyToValue.ContainsKey(fullKey) ? _keyToValue[fullKey].Value : null;
         }
      }

      public void Write(string key, string value)
      {
         lock (_watcherAndAccessLock)
         {
            if (_keyToValue.ContainsKey(key))
            {
               if (_keyToValue[key].Value == value) return;

               if (value == null)
               {
                  _keyToValue.Remove(key);
               }
               else
               {
                  _keyToValue[key].Value = value;
               }
            }
            else if (value != null)
            {
               _keyToValue[key] = new IniValue(value);
            }
            else
            {
               return;
            }

            //disables filewatcher because we are updating the file in code and not manually no point getting the file changed event
            _systemWatcher.EnableRaisingEvents = false;
            WriteIniFile();
            _systemWatcher.EnableRaisingEvents = true;
         }
      }

      private static bool TryGetKeyAndCategory(string fullKey, out string key, out string category)
      {
         int idx = fullKey.IndexOf(':');
         if (idx == -1)
         {
            key = fullKey;
            category = null;
         }
         else
         {
            key = fullKey.Substring(0, idx);
            category = fullKey.Substring(idx + 1);
         }
         return true;
      }

      private void ReadIniFile()
      {
         _keyToValue.Clear();
         FileInfo iniFile = new FileInfo(_fullName);
         if (iniFile.Exists)
         {
            using (var sr = new StreamReader(iniFile.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
            {
               string line;
               while ((line = sr.ReadLine()) != null)
               {
                  KeyValue kvp = new KeyValue(line);
                  if (kvp.Key != null) _keyToValue[kvp.Key] = new IniValue(kvp.Value);
               }
            }
         }
      }

      private void WriteIniFile()
      {
         var sb = new StringBuilder();
         //Clone the original so to work with the cloned dictionary.
         var localKeyValue = new Dictionary<string, IniValue>(_keyToValue);

         //Read values to retain format of the file
         using (var sr = new StreamReader(new FileStream(_fullName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read)))
         {
            string line;
            while ((line = sr.ReadLine()) != null)
            {
               KeyValue kvp = new KeyValue(line);
               if (kvp.Key != null)
               {
                  if (localKeyValue.ContainsKey(kvp.Key))
                  {
                     IniValue outValue;
                     if (localKeyValue.TryGetValue(kvp.Key, out outValue))
                     {
                        sb.AppendLine(string.Format("{0}={1}", kvp.Key, outValue.ToString()));
                        localKeyValue.Remove(kvp.Key);
                     }
                  }
               }
               else
               {
                  sb.AppendLine(line);
               }
            }
         }

         //Now get rest of the properties
         foreach (KeyValuePair<string, IniValue> line in localKeyValue)
         {
            if (line.Value.Value != null)
            {
               sb.Append(string.Format("{0}={1}", line.Key, line.Value));
            }
         }

         using (var w = new StreamWriter(_fullName))
         {
            w.Write(sb.ToString());
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
