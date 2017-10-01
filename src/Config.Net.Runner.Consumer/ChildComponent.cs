using Config.Net.Runner.Interfaces;

namespace Config.Net.Runner.Consumer
{
   public class ChildComponent
   {
      private readonly IConsoleCommands _cfg;

      public ChildComponent(IConsoleCommands cfg)
      {
         _cfg = cfg;
      }

      public override string ToString()
      {
         return $"I know that the config for Mode is {_cfg.Mode}!";
      }
   }
}