using System;
using System.IO;
using System.Reflection;
using NetBox.Extensions;

namespace Config.Net.Tests
{
   public class AbstractTestFixture
   {
      private string TestDirContainer = "ts" + Guid.NewGuid();
      private DirectoryInfo _testDir;
      private static bool cleanedUp = false;

      /// <summary>
      /// Isolated directory will be created for every test only when needed, and destroyed automagically
      /// </summary>
      protected DirectoryInfo TestDir
      {
         get
         {
            if (_testDir == null)
            {
               //Cleanup();

               string testDir = Path.Combine(BuildDir.FullName, TestDirContainer, Guid.NewGuid().ToString());
               Directory.CreateDirectory(testDir);
               _testDir = new DirectoryInfo(testDir);
            }
            return _testDir;
         }
      }

      public AbstractTestFixture()
      {
         if (cleanedUp) return;

         string dirPath = Path.Combine(BuildDir.FullName, TestDirContainer);

         if (Directory.Exists(dirPath)) Directory.Delete(dirPath, true);

         cleanedUp = true;
      }

      private void Cleanup()
      {
         //FS cleanup
         foreach (DirectoryInfo oldDir in BuildDir.GetDirectories(TestDirContainer + "*", SearchOption.TopDirectoryOnly))
         {
            oldDir.Delete(true);
         }
         _testDir = null;
      }

      protected DirectoryInfo BuildDir
      {
         get
         {
            return new DirectoryInfo(Path.GetDirectoryName(ThisAssembly.Location));
         }
      }


      private static Assembly _thisAsm;
      internal static Assembly ThisAssembly
      {
         get
         {
            if (_thisAsm == null)
            {
               _thisAsm = Assembly.GetExecutingAssembly();
            }

            return _thisAsm;
         }
      }
   }
}
