using System;

namespace Config.Net.Runner.Interfaces
{
   public interface IConsoleCommandsMetadata: IConsoleCommands
   {
      [Option(DefaultValue = "interactive")]
      string Mode { get; }
   }
}