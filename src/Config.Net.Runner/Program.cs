using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Config.Net.Runner
{

   public interface IConsoleCommands
   {
      [Option(DefaultValue = "download")]
      string Action { get; }

      string FilePath { get; }

      string AppKey { get; }

      string MyConnection { get; }

      [Option(Alias = "MySection.MyKey")]
      string MySectionKey { get; }

      [Option(Alias = "MySection.MyKey1")]
      string MySectionKey1 { get; }

      [Option(Alias = "MySection1.Key")]
      string MySectionKey2 { get; }
   }

   class Program
   {
      static void Main(string[] args)
      {
         IConsoleCommands settings =
            new ConfigurationBuilder<IConsoleCommands>()
            .UseAppConfig()
            .UseCommandLineArgs(
               new KeyValuePair<string, int>(nameof(IConsoleCommands.Action), 1),
               new KeyValuePair<string, int>(nameof(IConsoleCommands.FilePath), 2))
            .Build();

         Console.WriteLine("action: " + settings.Action + ", filePath: " + settings.FilePath);
         Console.WriteLine("appkey: {0}, myconn: {1}, custom key: {2}", settings.AppKey, settings.MyConnection, settings.MySectionKey);
         Console.WriteLine("key1: {0}, key2: {1}", settings.MySectionKey1, settings.MySectionKey2);

         Console.ReadKey();
      }
   }
}