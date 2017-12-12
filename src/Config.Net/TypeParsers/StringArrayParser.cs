using System;
using System.Collections.Generic;
using System.Linq;

namespace Config.Net.TypeParsers
{
   class StringArrayParser : ITypeParser
   {
      public IEnumerable<Type> SupportedTypes => new[] { typeof(string[]) };

      public bool TryParse(string value, Type t, out object result)
      {
         if (value == null)
         {
            result = null;
            return false;
         }

         result = ParseAsArray(value);
         return true;
      }

      public string ToRawString(object value)
      {
         string[] arv = (string[])value;

         if (arv == null || arv.Length == 0) return null;

         return string.Join(" ", arv.Select(Escape));
      }

      private static string Escape(string s)
      {
         string s1 = s.Replace("\"", "\"\"");

         return (s == s1 && !s.Contains(" "))
            ? s
            : $"\"{s1}\"";
      }

      private static string[] ParseAsArray(string s)
      {
         var a = new List<string>();
         string v = string.Empty;

         int state = 0;
         for(int i = 0; i < s.Length;)
         {
            char ch = s[i];

            switch(state)
            {
               case 0:     //default
                  if (ch == '\"')
                  {
                     state = 2;
                  }
                  else if(ch == ' ')
                  {
                     //skip spaces in default mode
                  }
                  else
                  {
                     v += ch;
                     state = 1;
                  }
                  i++;
                  break;
               case 1:     //reading unquoted value
                  if (ch == ' ')
                  {
                     a.Add(v);
                     v = string.Empty;
                     state = 0;
                  }
                  else
                  {
                     v += ch;
                  }
                  i++;
                  break;
               case 2:     //reading quoted value
                  if(ch == '\"')
                  {
                     a.Add(v);
                     v = string.Empty;
                     state = 0;
                  }
                  else
                  {
                     v += ch;
                  }
                  i++;
                  break;
            }
         }

         if(!string.IsNullOrEmpty(v))
         {
            a.Add(v);
         }

         return a.ToArray();
      }
   }
}
