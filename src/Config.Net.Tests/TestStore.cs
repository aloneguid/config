using System.Collections.Generic;

namespace Config.Net.Tests {
    class TestStore : IConfigStore {
        public Dictionary<string, string> Map { get; private set; }

        public TestStore() {
            Map = new Dictionary<string, string>();
        }

        public string Name { get { return "test"; } }
        public bool CanRead { get { return true; } }
        public bool CanWrite { get { return true; } }

        public string Read(string key) {
            if(Map.ContainsKey(key))
                return Map[key];
            return null;
        }

        public void Write(string key, string value) {
            Map[key] = value;
        }

        public void Dispose() {
        }
    }
}