using System;
using System.IO;
using System.Text;
using Config.Net.Stores.Formats.Ini;
using Xunit;

namespace Config.Net.Tests.Stores.Formats
{
   public class StructuredIniFileTest : AbstractTestFixture
   {
      [Fact]
      public void WriteInlineCommentWithNewLine()
      {
         string fullPath = Path.Combine(TestDir.FullName, Guid.NewGuid() + ".ini");
         string content = @"key=value ;c1\r\nc2
";

         StructuredIniFile file;
         using (Stream input = new MemoryStream(Encoding.UTF8.GetBytes(content)))
         {
            file = StructuredIniFile.ReadFrom(input, true, true);
         }

         using (Stream output = File.OpenWrite(fullPath))
         {
            file.WriteTo(output);
         }

         string resultText = File.ReadAllText(fullPath);
         Assert.Equal(content, resultText);
      }
   }
}
