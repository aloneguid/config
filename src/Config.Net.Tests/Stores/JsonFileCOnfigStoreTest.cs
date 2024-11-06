using System;
using System.Collections.Generic;
using System.IO;
using Config.Net.Stores;
using Xunit;

namespace Config.Net.Tests.Stores {
    public class JsonFileConfigStoreTest : AbstractTestFixture, IDisposable {
        private string _path;
        private JsonConfigStore _store;

        public JsonFileConfigStoreTest() {
            _path = Path.Combine(BuildDir.FullName, "TestData", "sample.json");
            _store = new JsonConfigStore(_path, true);
        }

        [Fact]
        public void Read_inline_property() {
            string key = _store.Read("ApplicationInsights.InstrumentationKey");

            Assert.NotNull(key);
        }

        [Fact]
        public void Write_inline_property_reads_back() {
            if(!_store.CanWrite)
                return;

            string key = "ApplicationInsights.InstrumentationKey";

            _store.Write(key, "123");

            Assert.Equal("123", _store.Read(key));
        }

        [Fact]
        public void Write_new_value_hierarchically() {
            if(!_store.CanWrite)
                return;

            string key = "One.Two.Three";

            _store.Write(key, "111");

            Assert.Equal("111", _store.Read(key));
        }

        [Fact]
        public void AliasesOnCollections() {
            IMyConfigUsingAliases myConfig = new ConfigurationBuilder<IMyConfigUsingAliases>()
               .UseConfigStore(_store)
               .Build();

            Assert.NotNull(myConfig.Credentials);
            foreach(ICredsWithAlias c in myConfig.Credentials) {
                if(c.Name == "user1") {
                    Assert.Equal("pass1", c.Pass);
                } else if(c.Name == "user2") {
                    Assert.Equal("pass2", c.Pass);
                } else {
                    Assert.Equal("user1", c.Name);
                }
            }
        }

        [Fact]
        public void TestCreatingFileInMissingFolder() {
            _path = Path.Combine("C:\\temp", "TestData", "sample.json");

            if(Directory.Exists(_path)) {
                Directory.Delete(_path);
            }

            string key = "One.Two.Three";

            _store = new JsonConfigStore(_path, true);
            _store.Write(key, "111");

            Assert.Equal("111", _store.Read(key));

            Assert.True(File.Exists(_path));
        }

        public void Dispose() {
            _store.Dispose();
        }
    }
    public interface ICredsWithAlias {
        [Option(Alias = "Username")]
        string Name { get; set; }
        [Option(Alias = "Password")]
        string Pass { get; set; }
    }

    public interface IMyConfigUsingAliases {
        [Option(Alias = "Creds")]
        IEnumerable<ICredsWithAlias> Credentials { get; }
    }
}