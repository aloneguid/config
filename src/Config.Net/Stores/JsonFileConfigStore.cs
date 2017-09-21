using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Config.Net.Stores
{
    /// <summary>
    /// Simple JSON storage.
    /// </summary>
    public class JsonFileConfigStore : IConfigStore
    {
        private readonly string _fullName;
        private readonly string _fileName;

        private JObject _jsonContent = new JObject();

        /// <summary>
        /// Create JSON storage in the file specified in <paramref name="pathName"/>.
        /// </summary>
        /// <param name="pathName">Full or relative path to JSON storage file.</param>
        /// <exception cref="ArgumentNullException"><paramref name="pathName"/> is null.</exception>
        /// <exception cref="IOException">Provided path is not valid.</exception>
        /// <remarks>Storage file does not have to exist, however it will be created as soon as first write performed.</remarks>
        public JsonFileConfigStore(string pathName)
        {
            if (pathName == null) throw new ArgumentNullException(nameof(pathName));

            _fullName = Path.GetFullPath(pathName);   // Allow relative path to JSON file
            _fileName = Path.GetFileName(_fullName);

            var parentDirPath = Path.GetDirectoryName(_fullName);

            if (string.IsNullOrEmpty(parentDirPath)) throw new IOException($"Provided directory path ({parentDirPath}) is not valid");

            Directory.CreateDirectory(parentDirPath);

            ReadJsonFile();
        }

        public void Dispose()
        {
            // nothing to dispose.
        }

        public string Name => $"json:{_fileName}";

        public bool CanRead => true;

        public bool CanWrite => true;

        public string Read(string key)
        {
            if (key == null)
                return null;

            ReadJsonFile();

            JToken token;
            _jsonContent.TryGetValue(key, out token);
            return token?.Value<string>();
        }

        public void Write(string key, string value)
        {
            _jsonContent[key] = value;

            WriteJsonFile();
        }

        private void ReadJsonFile()
        {
            var fileInfo = new FileInfo(_fullName);

            if (fileInfo.Exists)
            {
                using (var stream = fileInfo.OpenRead())
                using (var reader = new StreamReader(stream))
                using (var jsonReader = new JsonTextReader(reader))
                {
                    _jsonContent = JObject.Load(jsonReader);
                }
            }
            else
            {
                _jsonContent = new JObject();
            }
        }

        private void WriteJsonFile()
        {
            using (var fileStream = File.Create(_fullName))
            using (var writer = new StreamWriter(fileStream))
            using (var jsonWriter = new JsonTextWriter(writer))
            {
                _jsonContent.WriteTo(jsonWriter);
            }
        }
    }
}