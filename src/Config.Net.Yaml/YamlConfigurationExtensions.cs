using Config.Net.Yaml.Stores;

namespace Config.Net
{
   /// <summary>
   /// Configuration extensions
   /// </summary>
   public static class YamlConfigurationExtensions
   {
      public static ConfigurationBuilder<TInterface> UseYamlFile<TInterface>(this ConfigurationBuilder<TInterface> builder, string yamlFilePath) where TInterface : class
      {
         builder.UseConfigStore(new YamlFileConfigStore(yamlFilePath));
         return builder;
      }
   }
}
