using System;

namespace Config.Net.Stores
{
    public class EnvironmentVariablesStore : IConfigStore
    {
        public bool CanRead => true;

        public bool CanWrite => true;

        public string Name => "System Environment";


        public string Read(string key)
        {
            return Environment.GetEnvironmentVariable(key);
        }

        public void Write(string key, string value)
        {
            Environment.SetEnvironmentVariable(key, value);
        }

        public void Dispose()
        {
        }
    }
}