namespace Config.Net.Runner.Interfaces
{
   public static class Builder
   {
      public static IConsoleCommands Build()
      {
         return
            new ConfigurationBuilder<IConsoleCommandsMetadata>()
               .UseCommandLineArgs()
               .Build();
      }
   }
}