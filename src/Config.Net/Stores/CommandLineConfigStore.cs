﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Config.Net.Stores
{
   class CommandLineConfigStore : IConfigStore
   {
      private readonly Dictionary<string, string> _nameToValue = new Dictionary<string, string>();
      private static readonly char[] ArgPrefixes = new[] { '-', '/' };
      private static readonly string[] ArgDelimiters = new[] { ":", "=" };


      public bool CanRead => true;

      public bool CanWrite { get => false; set {  } }

      public string Name => "Command Line Arguments";

      public CommandLineConfigStore(string[] args,
         Dictionary<int, Option> positionToOption)
      {
         Parse(args ?? Environment.GetCommandLineArgs(), positionToOption);
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

      private void Parse(string[] args, Dictionary<int, Option> positionToOption)
      {
         _nameToValue.Clear();

         if (args == null) return;

         for(int i = 0; i < args.Length; i++)
         {
            string name;
            string value;

            if(positionToOption != null && positionToOption.TryGetValue(i, out Option option))
            {
               name = option.Name;
               value = args[i];
            }
            else
            {
               var nameValue = args[i].SplitByDelimiter(ArgDelimiters);
               name = nameValue.Item1.TrimStart(ArgPrefixes);
               value = nameValue.Item2;
            }

            if(name != null && value != null)
            {
               _nameToValue[name] = value;
            }
         }
      }
   }
}
