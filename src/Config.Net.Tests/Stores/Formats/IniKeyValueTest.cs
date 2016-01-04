using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Config.Net.Stores.Formats;

namespace Config.Net.Tests.Stores.Formats
{
   [TestFixture]
   public class IniKeyValueTest
   {
      [TestCase("key=value", "key", "value")]
      [TestCase("key=value;123", "key", "value")]
      [TestCase("key==value", "key", "=value")]
      [TestCase("key=value;value;value", "key", "value;value")]
      public void FromLine_Variable_Variable(string input, string expectedKey, string expectedValue)
      {
         IniKeyValue kv = IniKeyValue.FromLine(input);

         Assert.AreEqual(expectedKey, kv.Key);
         Assert.AreEqual(expectedValue, kv.Value);
      }
   }
}
