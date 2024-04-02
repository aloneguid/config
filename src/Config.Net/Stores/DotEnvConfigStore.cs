using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Config.Net.Core;
using Config.Net.Stores.Formats.Ini;

namespace Config.Net.Stores {
    public class DotEnvConfigStore : IConfigStore {

        private const string DotEnvFileName = ".env";
        private readonly StructuredIniFile? _iniFile;

        public DotEnvConfigStore(string? filePath = null) {
            _iniFile = TryBuildFile(filePath);
        }

        private static StructuredIniFile? TryBuildFile(string? filePath) {
            if(filePath == null) {
                filePath = Environment.CurrentDirectory;
            }

            var current = new DirectoryInfo(filePath);
            while(current != null) {
                string currentFilePath = Path.Combine(current.FullName, DotEnvFileName);
                if(File.Exists(currentFilePath)) {
                    using(FileStream stream = File.Open(currentFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
                        return StructuredIniFile.ReadFrom(stream, false);
                    }
                }

                current = current.Parent;
            }

            return null;
        }

        public bool CanRead => true;

        public bool CanWrite => false;

        public void Dispose() { }

        public string? Read(string key) {
            if(_iniFile == null) {
                return null;
            }

            if(FlatArrays.IsArrayLength(key, k => _iniFile[k], out int length)) {
                return length.ToString();
            }

            if(FlatArrays.IsArrayElement(key, k => _iniFile[k], out string? element)) {
                return element;
            }

            return _iniFile[key];
        }

        public void Write(string key, string? value) => throw new NotSupportedException();
    }
}