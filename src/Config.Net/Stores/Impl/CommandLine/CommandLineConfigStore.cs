#if !NETSTANDARD14
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Config.Net.Stores.Impl.CommandLine
{
   class CommandLineConfigStore : IConfigStore
   {
      private readonly Dictionary<string, string> _nameToValue;
      private static readonly char[] ArgPrefixes = new[] { '-', '/' };
      private static readonly string[] ArgDelimiters = new[] { "=" };
      private readonly bool _isCaseSensitive;

      public bool CanRead => true;

      public bool CanWrite => false;

      public CommandLineConfigStore(string[] args = null, bool isCaseSensitive = false, IEnumerable<KeyValuePair<string, int>> nameToPosition = null)
      {
         _isCaseSensitive = isCaseSensitive;

         _nameToValue = new Dictionary<string, string>(_isCaseSensitive ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase);

         Parse(args ?? Environment.GetCommandLineArgs(), nameToPosition);
      }

      public void Dispose()
      {
      }

      public string Read(string key)
      {
         if (key == null) return null;

         string value;
         _nameToValue.TryGetValue(key, out value);
         return value;
      }

      public void Write(string key, string value)
      {
         throw new NotSupportedException();
      }

      private void Parse(string[] args, IEnumerable<KeyValuePair<string, int>> nameToPosition)
      {
         _nameToValue.Clear();

         var posToName = new Dictionary<int, string>();
         if (nameToPosition != null)
         {
            foreach(KeyValuePair<string, int> p in nameToPosition)
            {
               if (p.Key != null)
               {
                  posToName[p.Value] = p.Key;
               }
            }
         }

         if (args == null) return;

         for (int i = 0; i < args.Length; i++)
         {
            string name;
            string value;

            Tuple<string, string> nameValue = Utils.SplitByDelimiter(args[i], ArgDelimiters);
            name = nameValue.Item1.TrimStart(ArgPrefixes);
            value = nameValue.Item2;

            if (name != null && value != null)
            {
               _nameToValue[name] = value;
            }
            else if(name != null && posToName.TryGetValue(i, out string ptnName))
            {
               _nameToValue[ptnName] = args[i];
            }
         }
      }
   }
}
#endif