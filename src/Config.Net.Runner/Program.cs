using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Config.Net.Runner
{

   public interface IConsoleCommands
   {
      [Option(DefaultValue = "interactive")]
      string Mode { get; }
   }

   class Program
   {
      static void Main(string[] args)
      {
         IConsoleCommands settings =
            new ConfigurationBuilder<IConsoleCommands>()
            .UseCommandLineArgs()
            .Build();

         Console.WriteLine("mode: " + settings.Mode);
      }
   }
}