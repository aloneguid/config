using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Config.Net.Core;
using YamlDotNet.RepresentationModel;

namespace Config.Net.Yaml.Stores
{
   public class YamlFileConfigStore : IConfigStore
   {
      private static readonly char[] HierarchySeparator = new[] {'.'};

      private readonly string _fullName;


      public YamlFileConfigStore(string fullName)
      {
         _fullName = Path.GetFullPath(fullName) ?? throw new ArgumentNullException(nameof(fullName));
      }

      public string Name => "yaml";

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
         if (name == null || !File.Exists(_fullName)) return null;

         bool isLength = OptionPath.TryStripLength(name, out name);

         var ys = new YamlStream();

         using (FileStream fs = File.OpenRead(_fullName))
         using (var reader = new StreamReader(fs))
         {
            ys = new YamlStream();
            ys.Load(reader);
         }

         string[] parts = name.Split(HierarchySeparator, StringSplitOptions.RemoveEmptyEntries);

         YamlNode current = ys.Documents[0].RootNode;

         foreach (string part in parts)
         {
            current = DiveIn(current, part);

            if (current == null) break;
         }

         return GetResult(current, isLength);
      }

      private YamlNode DiveIn(YamlNode node, string name)
      {
         bool isIndex = OptionPath.TryStripIndex(name, out name, out int index);

         if (node is YamlMappingNode currentMapping)
         {
            YamlNode result = currentMapping
               .Children
               .Where(c => IsMatch(c.Key, name))
               .Select(c => c.Value)
               .FirstOrDefault();

            if(isIndex && result is YamlSequenceNode sequenceNode)
            {
               if(index < sequenceNode.Count())
               {
                  result = sequenceNode[index];
               }
               else
               {
                  result = null;
               }
            }

            return result;
         }
         else if (node is YamlSequenceNode currentSequence)
         {
            YamlNode start = currentSequence.FirstOrDefault(el => DiveIn(el, name) != null);

            return DiveIn(start, name);
         }
         else
         {
            return null;
         }
      }

      private bool IsMatch(YamlNode node, string name)
      {
         if (node is YamlScalarNode scalar)
            return scalar.Value == name;

         return false;
      }

      private string GetResult(YamlNode node, bool isLength)
      {
         if (node == null) return null;

         if(isLength)
         {
            if(node is YamlSequenceNode sequenceNode)
            {
               return sequenceNode.Count().ToString();
            }

            return "0";
         }

         if (node is YamlScalarNode scalar)
         {
            return scalar.Value;
         }

         return null;
      }

   }
}
