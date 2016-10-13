using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace Config.Net.Tests
{
   public class AbstractTestFixture
   {
      private const string TestDirPrefix = "UNIT-TEST-";
      private const string TestStorageDirName = "TEST-STATE";
      private DirectoryInfo _testDir;

#if NETFULL
      static AbstractTestFixture()
      {
         /*Log.Configuration.LogFormat = LogFormat.ModernMini;
         Log.Configuration.Enable(LogType.ColoredConsole);
         Log.Configuration.Enable(LogType.Trace);*/

         ServicePointManager.ServerCertificateValidationCallback += CertificateValidationCallback;
      }

      private static bool CertificateValidationCallback(
         object sender,
         X509Certificate certificate,
         X509Chain chain,
         SslPolicyErrors sslPolicyErrors)
      {
         return true;
      }
#endif


      /// <summary>
      /// Isolated directory will be created for every test only when needed, and destroyed automagicaly
      /// </summary>
      protected DirectoryInfo TestDir
      {
         get
         {
            if(_testDir == null)
            {
               //Cleanup();

               string testDir = Path.Combine(BuildDir.FullName, TestDirPrefix + Guid.NewGuid());
               Directory.CreateDirectory(testDir);
               _testDir = new DirectoryInfo(testDir);
            }
            return _testDir;
         }
      }

      private void Cleanup()
      {
         //FS cleanup
         foreach(DirectoryInfo oldDir in BuildDir.GetDirectories(TestDirPrefix + "*", SearchOption.TopDirectoryOnly))
         {
            oldDir.Delete(true);
         }
         _testDir = null;
      }

      protected DirectoryInfo BuildDir
      {
         get
         {
            return NetPath.ExecDirInfo;
         }
      }
   }
}
