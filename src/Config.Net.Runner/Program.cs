using System;
using System.Diagnostics;
using Config.Net.Runner.Consumer;
using static Config.Net.Runner.Interfaces.Builder;

namespace Config.Net.Runner
{
   class Program
   {
      static void Main(string[] args)
      {
         var settings = Build();         
         Console.WriteLine("mode: " + settings.Mode);

         var clientComponent = new ChildComponent(settings);
         Console.WriteLine(clientComponent.ToString());
      }
   }
}