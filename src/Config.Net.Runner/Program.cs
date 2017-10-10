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
   }

   class Program
   {
      static void Main(string[] args)
      {
         IConsoleCommands settings =
            new ConfigurationBuilder<IConsoleCommands>()
            .UseCommandLineArgs(
               new KeyValuePair<string, int>(nameof(IConsoleCommands.Action), 1),
               new KeyValuePair<string, int>(nameof(IConsoleCommands.FilePath), 2))
            .Build();

         Console.WriteLine("action: " + settings.Action + ", filePath: " + settings.FilePath);

         Console.ReadKey();
      }
   }
}