using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Config.Net.Runner
{

   public class Cmds : SettingsContainer
   {
      public Option<string> Mode { get; } = new Option<string>("interactive");

      protected override void OnConfigure(IConfigConfiguration configuration)
      {
         configuration.UseCommandLineArgs(new Dictionary<int, Option> { { 0, Mode } });
      }
   }


   class Program
   {

      static void Main(string[] args)
      {
         var settings = new Cmds();

         Console.WriteLine("mode: " + settings.Mode);
      }
   }
}