using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using YamlDotNet.RepresentationModel;

namespace Config.Net.Stores
{
   class YamlFileConfigStore : IConfigStore
   {
      private static readonly char[] HierarchySeparator = new[] {'.'};

      private readonly string _fullName;


      public YamlFileConfigStore(string fullName)
      {
         _fullName = fullName ?? throw new ArgumentNullException(nameof(fullName));
      }

      public string Name => $"yaml";

      public bool CanRead => true;

      public bool CanWrite => false;

      public string Read(string key)
      {
         return ReadYamlKey(key);
      }

      public void Write(string key, string value)
      {
         throw new NotSupportedException();
      }

      public void Dispose()
      {
      }

      private string ReadYamlKey(string name)
      {
         if (!File.Exists(_fullName)) return null;

         var ys = new YamlStream();

         using (var fs = File.OpenRead(_fullName))
         using (var reader = new StreamReader(fs))
         {
            ys = new YamlStream();
            ys.Load(reader);
         }

         string[] parts = name.Split(HierarchySeparator, StringSplitOptions.RemoveEmptyEntries);

         YamlNode current = ys.Documents[0].RootNode;

         foreach (string part in parts)
         {
            if (current is YamlMappingNode currentMapping)
            {
               var currentFind = currentMapping
                  .Children
                  .FirstOrDefault(c => (c.Key is YamlScalarNode keyName) && (keyName.Value == part));

               current = currentFind.Value;
            }
            else
            {
               return null;
            }
         }

        if (current is YamlScalarNode currentScalar) return currentScalar.Value;

         throw new Exception("invalid YAML");
      }

   }
}
