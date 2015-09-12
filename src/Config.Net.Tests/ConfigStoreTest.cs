using Config.Net.Azure;
using NUnit.Framework;

namespace Config.Net.Tests
{
   [TestFixture("azure")]
   public class ConfigStoreTest
   {
      private readonly string _name;
      private IConfigStore _store;

      public ConfigStoreTest(string name)
      {
         _name = name;
      }

      [SetUp]
      public void SetUp()
      {
         switch(_name)
         {
            case "azure":
               _store = new AzureConfigStore();
               break;
         }
      }

      [Test]
      public void Smoke_Write_Reads()
      {
         if(!_store.CanWrite) Assert.Ignore();

         _store.Write("tk", "tv");

         Assert.AreEqual("tv", _store.Read("tk"));
      }

      [Test]
      public void Smoke_Read_Null()
      {
         Assert.IsNull(_store.Read("tk"));
      }
   }
}
